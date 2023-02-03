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

using System;
using System.Collections.Generic;
using System.Windows;
using D3BEmu.Common.Logging;
using D3BEmu.Common.MPQ;
using D3BEmu.Core.GS.Actors;
using D3BEmu.Core.GS.Common.Types.Collision;
using D3BEmu.Core.GS.Common.Types.Math;
using D3BEmu.Core.GS.Common.Types.SNO;
using D3BEmu.Core.GS.Common.Types.Scene;
using D3BEmu.Core.GS.Objects;
using D3BEmu.Core.GS.Players;
using D3BEmu.Net.GS.Message.Definitions.Map;
using D3BEmu.Net.GS.Message.Definitions.Scene;
using D3BEmu.Common.Helpers.Math;

namespace D3BEmu.Core.GS.Map
{

    public sealed class Scene : WorldObject
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        /// <summary>
        /// SNOHandle for the scene.
        /// </summary>
        public SNOHandle SceneSNO { get; private set; }

        /// <summary>
        /// Scene group's SNOId.
        /// Not sure on usage /raist.
        /// </summary>
        public int SceneGroupSNO { get; set; }

        /// <summary>
        /// Subscenes.
        /// </summary>
        public List<Scene> Subscenes { get; private set; }

        /// <summary>
        /// Parent scene if any.
        /// </summary>
        public Scene Parent;

        /// <summary>
        /// Parent scene's chunk id.
        /// </summary>
        public uint ParentChunkID
        {
            get { return (this.Parent != null) ? this.Parent.DynamicID : 0xFFFFFFFF; }
        }

        /// <summary>
        /// Visibility in MiniMap.
        /// </summary>
        public SceneMiniMapVisibility MiniMapVisibility { get; set; }

        /// <summary>
        /// Scene Specification.
        /// </summary>
        public SceneSpecification Specification { get; set; }

        /// <summary>
        /// Applied labels.
        /// Not sure on usage /raist.
        /// </summary>
        public int[] AppliedLabels;

        /// <summary>
        /// PRTransform for the scene.
        /// </summary>
        public PRTransform Transform
        {
            get { return new PRTransform { Quaternion = new Quaternion { W = this.RotationW, Vector3D = this.RotationAxis }, Vector3D = this.Position }; }
        }

        /// <summary>
        /// AABB bounds for the scene.
        /// </summary>
        public AABB AABBBounds { get; private set; }

        /// <summary>
        /// AABB bounds for MarketSet.
        /// </summary>
        public AABB AABBMarketSetBounds { get; private set; }

        /// <summary>
        /// NavMesh for the scene.
        /// </summary>
        public D3BEmu.Common.MPQ.FileFormats.Scene.NavMeshDef NavMesh { get; private set; }

        /// <summary>
        /// Markersets for the scene.
        /// </summary>
        public List<int> MarkerSets { get; private set; }

        /// <summary>
        /// LookLink - not sure on the usage /raist.
        /// </summary>
        public string LookLink { get; private set; }

        /// <summary>
        /// NavZone for the scene.
        /// </summary>
        public D3BEmu.Common.MPQ.FileFormats.Scene.NavZoneDef NavZone { get; private set; }

        /// <summary>
        /// Possible spawning locations for randomized gizmo placement
        /// </summary>
        public List<PRTransform>[] GizmoSpawningLocations { get; private set; }

        /// <summary>
        /// Creates a new scene and adds it to given world.
        /// </summary>
        /// <param name="world">The parent world.</param>
        /// <param name="position">The position.</param>
        /// <param name="snoId">SNOId for the scene.</param>
        /// <param name="parent">The parent scene if any.</param>
        public Scene(World world, Vector3D position, int snoId, Scene parent)
            : base(world, world.NewSceneID)
        {
            this.SceneSNO = new SNOHandle(SNOGroup.Scene, snoId);
            this.Parent = parent;
            this.Subscenes = new List<Scene>();
            this.Scale = 1.0f;
            this.AppliedLabels = new int[0];
            this.LoadSceneData(); // load data from mpqs.
            this.Size = new Size(this.NavZone.V0.X * this.NavZone.Float0, this.NavZone.V0.Y * this.NavZone.Float0);
            this.Position = position;
            this.World.AddScene(this); // add scene to the world.
        }

        #region mpq-data

        /// <summary>
        /// Loads scene data from mpqs.
        /// </summary>
        private void LoadSceneData()
        {
            // oh yeah, this really happens to me sometimes, i dont know why! ~weltmeyer
            if (!MPQStorage.Data.Assets[SNOGroup.Scene].ContainsKey(this.SceneSNO.Id))
                Logger.Debug("AssetsForScene not found in MPQ Storage:Scene:{0}, Asset:{1}", SNOGroup.Scene, this.SceneSNO.Id);
            var data = MPQStorage.Data.Assets[SNOGroup.Scene][this.SceneSNO.Id].Data as D3BEmu.Common.MPQ.FileFormats.Scene;
            if (data == null) return;

            this.AABBBounds = data.AABBBounds;
            this.AABBMarketSetBounds = data.AABBMarketSetBounds;
            this.NavMesh = data.NavMesh;
            this.MarkerSets = data.MarkerSets;
            this.LookLink = data.LookLink;
            this.NavZone = data.NavZone;
        }

        #endregion

        #region range-queries

        public List<Player> Players
        {
            get { return this.GetObjects<Player>(); }
        }

        public bool HasPlayers
        {
            get { return this.Players.Count > 0; }
        }

        public List<Actor> Actors
        {
            get { return this.GetObjects<Actor>(); }
        }

        public bool HasActors
        {
            get { return this.Actors.Count > 0; }
        }

        public List<T> GetObjects<T>() where T : WorldObject
        {
            return this.World.QuadTree.Query<T>(this.Bounds);
        }

        #endregion

        #region actor-loading

        /// <summary>
        /// Loads all markers for the scene.        
        /// </summary>
        public void LoadMarkers()
        {
            this.GizmoSpawningLocations = new List<PRTransform>[26]; // LocationA to LocationZ

            // TODO: We should be instead loading actors but let them get revealed based on quest/triggers/player proximity. /raist.

            foreach (var markerSet in this.MarkerSets)
            {
                var markerSetData = MPQStorage.Data.Assets[SNOGroup.MarkerSet][markerSet].Data as D3BEmu.Common.MPQ.FileFormats.MarkerSet;
                if (markerSetData == null) return;

                foreach (var marker in markerSetData.Markers)
                {
                    switch (marker.Type)
                    {
                        case D3BEmu.Common.MPQ.FileFormats.MarkerType.AmbientSound:
                        case D3BEmu.Common.MPQ.FileFormats.MarkerType.Light:
                        case D3BEmu.Common.MPQ.FileFormats.MarkerType.Particle:
                        case D3BEmu.Common.MPQ.FileFormats.MarkerType.SubScenePosition:
                        case D3BEmu.Common.MPQ.FileFormats.MarkerType.AudioVolume:
                            // nothing to do for these here, client load them on its own
                            break;

                        case D3BEmu.Common.MPQ.FileFormats.MarkerType.Script:
                            Logger.Trace("Ignoring marker {0} in {1} ({2}) because scripts are not handled yet", marker.Name, markerSetData.FileName, markerSetData.Header.SNOId);
                            break;

                        case D3BEmu.Common.MPQ.FileFormats.MarkerType.Event:
                            Logger.Trace("Ignoring marker {0} in {1} ({2}) because events are not handled yet", marker.Name, markerSetData.FileName, markerSetData.Header.SNOId);
                            break;

                        case D3BEmu.Common.MPQ.FileFormats.MarkerType.MinimapMarker:
                            Logger.Trace("Ignoring marker {0} in {1} ({2}) because minimap marker are not handled yet", marker.Name, markerSetData.FileName, markerSetData.Header.SNOId);

                            break;

                        case D3BEmu.Common.MPQ.FileFormats.MarkerType.Actor:

                            var actor = ActorFactory.Create(this.World, marker.SNOHandle.Id, marker.TagMap); // try to create it.
                            if (actor == null) continue;

                            var position = marker.PRTransform.Vector3D + this.Position; // calculate the position for the actor.
                            actor.RotationW = marker.PRTransform.Quaternion.W;
                            actor.RotationAxis = marker.PRTransform.Quaternion.Vector3D;

                            actor.EnterWorld(position);
                            break;

                        case D3BEmu.Common.MPQ.FileFormats.MarkerType.Encounter:

                            var encounter = marker.SNOHandle.Target as D3BEmu.Common.MPQ.FileFormats.Encounter;
                            var actorsno = RandomHelper.RandomItem(encounter.Spawnoptions, x => x.Probability);
                            var actor2 = ActorFactory.Create(this.World, actorsno.SNOSpawn, marker.TagMap); // try to create it.
                            if (actor2 == null) continue;

                            var position2 = marker.PRTransform.Vector3D + this.Position; // calculate the position for the actor.
                            actor2.RotationW = marker.PRTransform.Quaternion.W;
                            actor2.RotationAxis = marker.PRTransform.Quaternion.Vector3D;

                            actor2.EnterWorld(position2);

                            break;

                        default:

                            // Save gizmo locations. They are used to spawn loots and gizmos randomly in a level area
                            if ((int)marker.Type >= (int)D3BEmu.Common.MPQ.FileFormats.MarkerType.GizmoLocationA && (int)marker.Type <= (int)D3BEmu.Common.MPQ.FileFormats.MarkerType.GizmoLocationZ)
                            {
                                int index = (int)marker.Type - 50; // LocationA has id 50...

                                if (GizmoSpawningLocations[index] == null)
                                    GizmoSpawningLocations[index] = new List<PRTransform>();

                                marker.PRTransform.Vector3D += this.Position;
                                GizmoSpawningLocations[index].Add(marker.PRTransform);
                            }
                            else
                                Logger.Warn("Unhandled marker type {0} in actor loading", marker.Type);


                            break;

                    }
                }
            }
        }

        #endregion

        #region scene revealing & unrevealing

        /// <summary>
        /// Returns true if the actor is revealed to player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns><see cref="bool"/></returns>
        public bool IsRevealedToPlayer(Player player)
        {
            return player.RevealedObjects.ContainsKey(this.DynamicID);
        }

        /// <summary>
        /// Reveal the scene to given player.
        /// </summary>
        /// <param name="player">Player to reveal scene.</param>
        /// <returns></returns>
        public override bool Reveal(Player player)
        {
            if (player.RevealedObjects.ContainsKey(this.DynamicID)) return false; // return if already revealed.

            player.InGameClient.SendMessage(this.RevealMessage, true); // reveal the scene itself.
            player.InGameClient.SendMessage(this.MapRevealMessage, true); // reveal the scene in minimap.

            foreach (var sub in this.Subscenes) // reveal subscenes too.
            {
                sub.Reveal(player);
            }

            player.RevealedObjects.Add(this.DynamicID, this);
            return true;
        }

        /// <summary>
        /// Unreveals the scene to given player.
        /// </summary>
        /// <param name="player">Player to unreveal scene.</param>
        /// <returns></returns>
        public override bool Unreveal(Player player)
        {
            if (!player.RevealedObjects.ContainsKey(this.DynamicID)) return false; // if it's not revealed already, just return.

            player.InGameClient.SendMessage(new DestroySceneMessage() { WorldID = this.World.DynamicID, SceneID = this.DynamicID }, true); // send the unreveal message.

            foreach (var subScene in this.Subscenes) // unreveal subscenes too.
            {
                subScene.Unreveal(player);
            }

            player.RevealedObjects.Remove(this.DynamicID);
            return true;
        }

        #endregion

        #region scene-related messages

        /// <summary>
        /// Returns a RevealSceneMessage.
        /// </summary>
        public RevealSceneMessage RevealMessage
        {
            get
            {
                SceneSpecification specification = this.Specification;
                specification.SNOMusic = World.Environment.snoMusic;
                specification.SNOCombatMusic = World.Environment.snoCombatMusic;
                specification.SNOAmbient = World.Environment.snoAmbient;
                specification.SNOReverb = World.Environment.snoReverb;
                specification.SNOWeather = World.Environment.snoWeather;

                return new RevealSceneMessage
                {
                    WorldID = this.World.DynamicID,
                    SceneSpec = specification,
                    ChunkID = this.DynamicID,
                    Transform = this.Transform,
                    SceneSNO = this.SceneSNO.Id,
                    ParentChunkID = this.ParentChunkID,
                    SceneGroupSNO = this.SceneGroupSNO,
                    arAppliedLabels = this.AppliedLabels
                };
            }
        }

        /// <summary>
        /// Returns a MapRevealSceneMessage.
        /// </summary>
        public MapRevealSceneMessage MapRevealMessage
        {
            get
            {
                return new MapRevealSceneMessage
                {
                    ChunkID = this.DynamicID,
                    SceneSNO = this.SceneSNO.Id,
                    Transform = this.Transform,
                    WorldID = this.World.DynamicID,
                    MiniMapVisibility = this.MiniMapVisibility
                };
            }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("[Scene] SNOId: {0} DynamicId: {1} Name: {2}", this.SceneSNO.Id, this.DynamicID, this.SceneSNO.Name);
        }
    }

    /// <summary>
    /// Minimap visibility of the scene on map.
    /// </summary>
    public enum SceneMiniMapVisibility
    {
        /// <summary>
        /// Hidden.
        /// </summary>
        Hidden = 0,
        /// <summary>
        /// Revealed to player.
        /// </summary>
        Revealed = 1,
        /// <summary>
        /// Player already visited the scene.
        /// </summary>
        Visited = 2
    }
}
