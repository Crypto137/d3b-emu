﻿/*
 * Copyright (C) 2023 d3b-emu
 *
 * This program is free software: you can redistribute it and/or modify 
 * it under the terms of the GNU Affero General Public License as published by 
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with this program; if not, see <https://www.gnu.org/licenses/>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using D3BEmu.Common.MPQ;
using D3BEmu.Core.GS.Common.Types.SNO;
using D3BEmu.Core.GS.Map;
using D3BEmu.Core.GS.Common.Types.TagMap;
using D3BEmu.Core.GS.Actors.Implementations;
using D3BEmu.Common.Logging;

namespace D3BEmu.Core.GS.Actors
{
    public static class ActorFactory
    {
        private static readonly Dictionary<int, Type> SNOHandlers = new Dictionary<int, Type>();
        private static Logger Logger = new Logger("ActorFactory");

        static ActorFactory()
        {
            LoadSNOHandlers();
        }

        public static Actor Create(World world, int snoId, TagMap tags)
        {
            if (!MPQStorage.Data.Assets[SNOGroup.Actor].ContainsKey(snoId))
                return null;

            var actorAsset = MPQStorage.Data.Assets[SNOGroup.Actor][snoId];
            var actorData = actorAsset.Data as D3BEmu.Common.MPQ.FileFormats.Actor;
            if (actorData == null) return null;

            if (actorData.Type == ActorType.Invalid)
                return null;

            // see if we have an implementation for actor.
            if (SNOHandlers.ContainsKey(snoId))
                return (Actor)Activator.CreateInstance(SNOHandlers[snoId], new object[] { world, snoId, tags });

            switch (actorData.Type)
            {
                case ActorType.Monster:
                    if (tags.ContainsKey(MarkerKeys.ConversationList))
                        return new InteractiveNPC(world, snoId, tags);
                    else
                        if (!MPQStorage.Data.Assets[SNOGroup.Monster].ContainsKey(actorData.MonsterSNO))
                        return null;

                    var monsterAsset = MPQStorage.Data.Assets[SNOGroup.Monster][actorData.MonsterSNO];
                    var monsterData = monsterAsset.Data as D3BEmu.Common.MPQ.FileFormats.Monster;
                    if (monsterData.Type == D3BEmu.Common.MPQ.FileFormats.Monster.MonsterType.Ally ||
                        monsterData.Type == D3BEmu.Common.MPQ.FileFormats.Monster.MonsterType.Helper)
                        return new NPC(world, snoId, tags);
                    else
                        return new Monster(world, snoId, tags);
                case ActorType.Gizmo:
                    switch (actorData.TagMap[ActorKeys.GizmoGroup])
                    {
                        case GizmoGroup.LootContainer:
                            return new LootContainer(world, snoId, tags);
                        case GizmoGroup.Door:
                            return new Door(world, snoId, tags);
                        case GizmoGroup.DestructibleLootContainer:
                        case GizmoGroup.Destructible:
                        case GizmoGroup.Passive:
                        case GizmoGroup.Barricade:
                            return new DestructibleLootContainer(world, snoId, tags);
                        case GizmoGroup.Portal:
                            return new Portal(world, snoId, tags);
                        case GizmoGroup.BossPortal:
                            Logger.Warn("Skipping loading of boss portals");
                            return null;
                        case GizmoGroup.CheckPoint:
                            return new Checkpoint(world, snoId, tags);
                        case GizmoGroup.Waypoint:
                            return new Waypoint(world, snoId, tags);
                        case GizmoGroup.Savepoint:
                            return new Savepoint(world, snoId, tags);
                        case GizmoGroup.ProximityTriggered:
                            return new ProximityTriggeredGizmo(world, snoId, tags);
                        case GizmoGroup.Shrine:
                            return new Shrine(world, snoId, tags);
                        case GizmoGroup.Healthwell:
                            return new Healthwell(world, snoId, tags);
                        case GizmoGroup.StartLocations:
                            return new StartingPoint(world, snoId, tags);

                        case GizmoGroup.GateGizmo:
                        case GizmoGroup.ActChangeTempObject:
                        case GizmoGroup.Banner:
                        case GizmoGroup.CathedralIdol:
                        case GizmoGroup.DungeonStonePortal:
                        case GizmoGroup.Headstone:
                        case GizmoGroup.HearthPortal:
                        case GizmoGroup.PlayerSharedStash:
                        case GizmoGroup.QuestLoot:
                        case GizmoGroup.Readable:
                        case GizmoGroup.ServerProp:
                        case GizmoGroup.Sign:
                        case GizmoGroup.Spawner:
                        case GizmoGroup.TownPortal:
                        case GizmoGroup.Trigger:
                        case GizmoGroup.ScriptObject:
                            Logger.Info("GizmoGroup {0} has no proper implementation, using default gizmo instead", actorData.TagMap[ActorKeys.GizmoGroup]);
                            return CreateGizmo(world, snoId, tags);
                        default:
                            Logger.Warn("Unknown gizmo group {0}", actorData.TagMap[ActorKeys.GizmoGroup]);
                            return CreateGizmo(world, snoId, tags);
                    }
                case ActorType.ServerProp:
                    return new ServerProp(world, snoId, tags);
            }
            return null;
        }

        private static Actor CreateGizmo(World world, int snoId, TagMap tags)
        {
            if (tags.ContainsKey(MarkerKeys.DestinationWorld))
                return new Portal(world, snoId, tags);

            return new Gizmo(world, snoId, tags);
        }

        public static void LoadSNOHandlers()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!type.IsSubclassOf(typeof(Actor))) continue;

                var attributes = (HandledSNOAttribute[])type.GetCustomAttributes(typeof(HandledSNOAttribute), true);
                if (attributes.Length == 0) continue;

                foreach (var sno in attributes.First().SNOIds)
                {
                    if (!SNOHandlers.ContainsKey(sno))
                        SNOHandlers.Add(sno, type);
                }
            }
        }
    }
}
