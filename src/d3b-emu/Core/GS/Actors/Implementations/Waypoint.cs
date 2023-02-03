/*
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

using System.Windows;
using D3BEmu.Common.MPQ;
using D3BEmu.Core.GS.Common.Types.SNO;
using D3BEmu.Core.GS.Map;
using D3BEmu.Core.GS.Players;
using D3BEmu.Net.GS.Message;
using D3BEmu.Net.GS.Message.Definitions.Animation;
using D3BEmu.Net.GS.Message.Definitions.Map;
using D3BEmu.Net.GS.Message.Definitions.Misc;
using D3BEmu.Net.GS.Message.Fields;
using D3BEmu.Core.GS.Common.Types.TagMap;

namespace D3BEmu.Core.GS.Actors.Implementations
{
    public sealed class Waypoint : Gizmo
    {
        public int WaypointId { get; private set; }

        public Waypoint(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Attributes[GameAttribute.MinimapActive] = true;
        }

        public override void OnEnter(World world)
        {
            this.ReadWaypointId();
        }

        private void ReadWaypointId()
        {
            var actData = (D3BEmu.Common.MPQ.FileFormats.Act)MPQStorage.Data.Assets[SNOGroup.Act][70015].Data;
            var wayPointInfo = actData.WayPointInfo;

            var proximity = new Rect(this.Position.X - 1.0, this.Position.Y - 1.0, 2.0, 2.0);
            var scenes = this.World.QuadTree.Query<Scene>(proximity);
            if (scenes.Count == 0) return; // TODO: fixme! /raist

            var scene = scenes[0]; // Parent scene /fasbat

            if (scenes.Count == 2) // What if it's a subscene? /fasbat
            {
                if (scenes[1].ParentChunkID != 0xFFFFFFFF)
                    scene = scenes[1];
            }

            for (int i = 0; i < wayPointInfo.Length; i++)
            {
                if (wayPointInfo[i].SNOLevelArea == -1)
                    continue;

                if (scene.Specification == null) continue;
                foreach (var area in scene.Specification.SNOLevelAreas)
                {
                    if (wayPointInfo[i].SNOWorld != this.World.WorldSNO.Id || wayPointInfo[i].SNOLevelArea != area)
                        continue;

                    this.WaypointId = i;
                    break;
                }
            }
        }

        public override void OnTargeted(Player player, Net.GS.Message.Definitions.World.TargetMessage message)
        {
            var world = player.World;

            world.BroadcastIfRevealed(new PlayAnimationMessage()
            {
                ActorID = this.DynamicID,
                Field1 = 5,
                Field2 = 0f,
                tAnim = new[]
                    {
                        new PlayAnimationMessageSpec()
                        {
                            Duration = 4,
                            AnimationSNO = 0x2f761,
                            PermutationIndex = 0,
                            Speed = 1f,
                        }
                    }
            }, this);

            player.InGameClient.SendMessage(new ANNDataMessage(Opcodes.OpenWaypointSelectionWindowMessage)
            {
                ActorID = this.DynamicID
            });
        }

        public override bool Reveal(Player player)
        {
            if (!base.Reveal(player))
                return false;

            // Show a minimap icon
            player.InGameClient.SendMessage(new MapMarkerInfoMessage()
            {
                Field0 = (int)World.NewActorID,    // TODO What is the correct id space for mapmarkers? /fasbat
                Field1 = new WorldPlace()
                {
                    Position = this.Position,
                    WorldID = this.World.DynamicID
                },
                Field2 = 0x1FA21,
                m_snoStringList = 0xF063,

                Field4 = unchecked((int)0x9799F57B),
                Field5 = 0,
                Field6 = 0,
                Field7 = 0,
                Field8 = 0,
                Field9 = true,
                Field10 = false,
                Field11 = false,
                Field12 = 0
            });

            return true;
        }
    }
}
