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

using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using D3BEmu.Common.Helpers.Concurrency;
using D3BEmu.Core.GS.Actors;
using D3BEmu.Core.GS.Players;

namespace D3BEmu.Core.GS.Map.Debug
{
    public class DebugNavMesh
    {
        public World World { get; private set; }
        public Player Player { get; private set; }
        public RectangleF Bounds { get { return World.QuadTree.RootNode.Bounds; } }

        public ConcurrentList<Scene> MasterScenes { get; private set; }
        public ConcurrentList<Scene> SubScenes { get; private set; }
        public ConcurrentList<RectangleF> UnWalkableCells { get; private set; }
        public ConcurrentList<RectangleF> WalkableCells { get; private set; }
        public ConcurrentList<Player> Players { get; private set; }
        public ConcurrentList<Monster> Monsters { get; private set; }
        public ConcurrentList<NPC> NPCs { get; private set; }

        public bool DrawMasterScenes;
        public bool DrawSubScenes;
        public bool DrawWalkableCells;
        public bool DrawUnwalkableCells;
        public bool DrawMonsters;
        public bool DrawNPCs;
        public bool DrawPlayers;
        public bool PrintSceneLabels;
        public bool FillCells;
        public bool DrawPlayerProximityCircle;
        public bool DrawPlayerProximityRectangle;

        private readonly Pen _masterScenePen = new Pen(Color.Black, 1.0f);
        private readonly Pen _subScenePen = new Pen(Color.DarkGray, 1.0f);
        private readonly Brush _unwalkableBrush = Brushes.Red;
        private readonly Pen _unwalkablePen = new Pen(Color.Red, 1.0f);
        private readonly Brush _walkableBrush = Brushes.Blue;
        private readonly Pen _walkablePen = new Pen(Color.Blue, 1.0f);
        private readonly Pen _playerProximityPen = new Pen(Brushes.DarkViolet, 2.0f);
        private readonly Font _sceneFont = new Font("Verdana", 7);        

        public DebugNavMesh(World world, Player player = null)
        {
            this.World = world;
            this.Player = player;

            this._subScenePen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;

            this.MasterScenes = new ConcurrentList<Scene>();
            this.SubScenes = new ConcurrentList<Scene>();
            this.UnWalkableCells = new ConcurrentList<RectangleF>();
            this.WalkableCells = new ConcurrentList<RectangleF>();
            this.Players = new ConcurrentList<Player>();
            this.Monsters = new ConcurrentList<Monster>();
            this.NPCs = new ConcurrentList<NPC>();           
        }

        #region update

        public void Update(bool processObjectsInAllTheWorld)
        {
            this.MasterScenes.Clear();
            this.SubScenes.Clear();
            this.WalkableCells.Clear();
            this.UnWalkableCells.Clear();
            this.Players.Clear();
            this.Monsters.Clear();
            this.NPCs.Clear();

            var scenes = (processObjectsInAllTheWorld || this.Player == null)
                                        ? World.QuadTree.Query<Scene>(World.QuadTree.RootNode.Bounds)
                                        : this.Player.GetScenesInRegion();

            Parallel.ForEach(scenes, scene =>
            {
                if (scene.Parent == null)
                    this.MasterScenes.Add(scene);
                else
                    this.SubScenes.Add(scene);

                this.AnalyzeScene(scene);
            });

            var actors = (processObjectsInAllTheWorld || this.Player == null)
                        ? World.QuadTree.Query<Actor>(World.QuadTree.RootNode.Bounds)
                        : this.Player.GetActorsInRange();

            Parallel.ForEach(actors, actor =>
            {
                if (actor is Player)
                    this.Players.Add(actor as Player);
                else if (actor is NPC)
                    this.NPCs.Add(actor as NPC);
                else if (actor is Monster)
                    this.Monsters.Add(actor as Monster);
            });
        }

        private void AnalyzeScene(Scene scene)
        {
            Parallel.ForEach(scene.NavZone.NavCells, cell =>
            {
                float x = scene.Position.X + cell.Min.X;
                float y = scene.Position.Y + cell.Min.Y;

                float sizex = cell.Max.X - cell.Min.X;
                float sizey = cell.Max.Y - cell.Min.Y;

                var rect = new RectangleF(x, y, sizex, sizey);

                // TODO: Feature request: Also allow drawing of NavCellFlags.NOSpawn, NavCellFlags.LevelAreaBit0, NavCellFlags.LevelAreaBit1 cells. /raist.

                if ((cell.Flags & D3BEmu.Common.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk) != D3BEmu.Common.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk)
                    UnWalkableCells.Add(rect);
                else
                    WalkableCells.Add(rect);
            });
        }

        #endregion

        #region drawing 

        public Bitmap Draw()
        {
            // As quad-tree always has 4 quad-nodes beneath the root node, the quad node's area will be far larger then actual area covered by scenes.
            // We don't want to draw a bitmap that's as large as quad-tree's area, as it'll be consuming so much memory.
            // So instead find the rightMostScene and bottomMostScene and limit the drawed bitmaps size according. /raist.
            // TODO: We can even limit to leftMostScene and topMostScene because player-proximity rendering mode will be also containing large empty areas. /raist.

            Scene rightMostScene = null;
            Scene bottomMostScene = null;

            foreach (var scene in this.MasterScenes)
            {
                if (rightMostScene == null)
                    rightMostScene = scene;

                if (bottomMostScene == null)
                    bottomMostScene = scene;

                if (scene.Bounds.X + scene.Bounds.Width > rightMostScene.Bounds.X + rightMostScene.Bounds.Width)
                    rightMostScene = scene;

                if (scene.Bounds.Y + scene.Bounds.Height > bottomMostScene.Bounds.Y + bottomMostScene.Bounds.Height)
                    bottomMostScene = scene;
            }

            if (rightMostScene == null || bottomMostScene == null) 
                return null;

            var maxX = (int) (rightMostScene.Bounds.X + rightMostScene.Bounds.Width) + 1;
            var maxY = (int)(bottomMostScene.Bounds.Y + bottomMostScene.Bounds.Height) + 1;

            var bitmap = new Bitmap(maxX, maxY, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);

            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;

                graphics.FillRectangle(Brushes.LightGray, 0, 0, bitmap.Width, bitmap.Height);

                this.DrawShapes(graphics);

                if (this.PrintSceneLabels)
                    this.DrawLabels(graphics);

                graphics.Save();
            }

            return bitmap;
        }

        private void DrawShapes(Graphics graphics)
        {
            if (this.DrawMasterScenes)
            {
                foreach (var scene in this.MasterScenes)
                {
                    this.DrawScene(graphics, scene);
                }
            }

            if (this.DrawSubScenes)
            {
                foreach (var scene in this.SubScenes)
                {
                    this.DrawScene(graphics, scene);
                }
            }

            if (this.DrawWalkableCells)
                this.DrawWalkables(graphics);

            if (this.DrawUnwalkableCells)
                this.DrawUnwalkables(graphics);

            if (this.DrawMonsters)
            {
                foreach (var monster in this.Monsters)
                {
                    this.DrawActor(monster, graphics, Brushes.Green, 7);
                }
            }

            if (this.DrawNPCs)
            {
                foreach (var npc in this.NPCs)
                {
                    this.DrawActor(npc, graphics, Brushes.Orange, 7);
                }
            }

            if (this.DrawPlayers)
            {
                foreach (var player in this.Players)
                {
                    this.DrawActor(player, graphics, Brushes.DarkViolet, 7);
                }
            }

            if(this.DrawPlayerProximityCircle)
                this.DrawProximityCircle(graphics);

            if(this.DrawPlayerProximityRectangle)
                this.DrawProximityRectangle(graphics);
        }

        private void DrawScene(Graphics graphics, Scene scene)
        {
            var rect = new Rectangle((int)scene.Bounds.Left, (int)scene.Bounds.Top, (int)scene.Bounds.Width, (int)scene.Bounds.Height);
            graphics.DrawRectangle(scene.Parent == null ? _masterScenePen : _subScenePen, rect);
        }

        private void DrawWalkables(Graphics graphics)
        {
            foreach (var cell in this.WalkableCells)
            {
                var rect = new Rectangle(new System.Drawing.Point((int)cell.Left, (int)cell.Top), new System.Drawing.Size((int)cell.Width, (int)cell.Height));
                
                if (this.FillCells)
                    graphics.FillRectangle(_walkableBrush, rect);
                else
                    graphics.DrawRectangle(_walkablePen, rect);
            }
        }

        private void DrawUnwalkables(Graphics graphics)
        {
            foreach (var cell in this.UnWalkableCells)
            {
                var rect = new Rectangle(new System.Drawing.Point((int)cell.Left, (int)cell.Top), new System.Drawing.Size((int)cell.Width, (int)cell.Height));
                
                if (this.FillCells)
                    graphics.FillRectangle(_unwalkableBrush, rect);
                else
                    graphics.DrawRectangle(_unwalkablePen, rect);
            }
        }

        private void DrawActor(Actor actor, Graphics graphics, Brush brush, int radius)
        {
            var rect = new Rectangle((int)actor.Bounds.X, (int)actor.Bounds.Y, (int)actor.Bounds.Width + radius, (int)actor.Bounds.Height + radius);
            graphics.FillEllipse(brush, rect);
        }

        private void DrawProximityCircle(Graphics graphics)
        {
            if (this.Player == null) 
                return;

            var rect = new RectangleF(this.Player.Position.X - Actor.DefaultQueryProximityRadius,
                                      this.Player.Position.Y - Actor.DefaultQueryProximityRadius,
                                      Actor.DefaultQueryProximityRadius * 2,
                                      Actor.DefaultQueryProximityRadius * 2);

            graphics.DrawEllipse(this._playerProximityPen, rect);
        }

        private void DrawProximityRectangle(Graphics graphics)
        {
            if (this.Player == null)
                return;

            graphics.DrawRectangle(this._playerProximityPen,
                                   this.Player.Position.X - Actor.DefaultQueryProximityLenght/2,
                                   this.Player.Position.Y - Actor.DefaultQueryProximityLenght/2,
                                   Actor.DefaultQueryProximityLenght,
                                   Actor.DefaultQueryProximityLenght);
        }

        private void DrawLabels(Graphics graphics)
        {
            if (this.DrawMasterScenes)
            {
                foreach (var scene in this.MasterScenes)
                {
                    this.DrawSceneLabel(graphics, scene);
                }
            }

            if (this.DrawSubScenes)
            {
                foreach (var scene in this.SubScenes)
                {
                    this.DrawSceneLabel(graphics, scene);
                }
            }
        }

        private void DrawSceneLabel(Graphics graphics, Scene scene)
        {
            var stringRectangle = new RectangleF((float)scene.Bounds.Left, (float)scene.Bounds.Top, (float)scene.Bounds.Width, (float)scene.Bounds.Height);
            var drawFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            if (!string.IsNullOrEmpty(scene.SceneSNO.Name))
                graphics.DrawString(scene.SceneSNO.Name, _sceneFont, scene.Parent == null ? Brushes.Black : Brushes.Gray, stringRectangle, drawFormat);
        }

        #endregion
    }
}
