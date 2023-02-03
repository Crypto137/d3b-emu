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

using System.Collections.Generic;
using System.Linq;
using D3BEmu.Core.GS.Actors.Actions;
using D3BEmu.Core.GS.Common.Types.Math;
using D3BEmu.Core.GS.Ticker;

namespace D3BEmu.Core.GS.Actors.Movement
{
    public class MoveToPointWithPathfindAction : ActorAction
    {
        public Vector3D Heading { get; private set; }

        public SteppedRelativeTickTimer Timer;
        private List<Vector3D> _path = new List<Vector3D>();
        private AI.Pather.PathRequestTask _pathRequestTask;
        public MoveToPointWithPathfindAction(Actor owner, Vector3D heading)
            : base(owner)
        {
            // Sending a request for a Path to the Pathing thread.
            _pathRequestTask = owner.World.Game.Pathfinder.GetPath(owner, owner.Position, heading);
            this.Heading = heading;

        }

        public override void Start(int tickCounter)
        {
            // Just wait, path request hasnt been processed yet, idealy this would be null or something instead - Darklotus
            if (!_pathRequestTask.PathFound)
                return;

            // No path found, so end Action.
            if (_pathRequestTask.Path.Count < 1)
            {
                this.Started = true;
                this.Done = true;
                return;
            }
            _path = _pathRequestTask.Path;
            // Each path step will be 2.5f apart roughly, not sure on the math to get correct walk speed for the timer.
            // mobs sometimes skip a bit, pretty sure this is because timing isnt correct.  :( - DarkLotus
                      

            this.Timer = new SteppedRelativeTickTimer(this.Owner.World.Game, 18, (int)(_path.Count *2 / this.Owner.WalkSpeed),
            (tick) =>
            {
                //this.Owner.Position = MovementHelpers.GetMovementPosition(this.Owner.Position, this.Owner.WalkSpeed, facingAngle, 6);
                if (_path.Count >= 1)
                {
                    this.Owner.Move(this._path.First(), MovementHelpers.GetFacingAngle(this.Owner, this._path.First()));
                    this.Owner.Position = _path.First();
                    _path.RemoveAt(0);
                    //Logger.Trace("Step left in Queue: " + Path.Count);
                }
                else
                {
                    this.Owner.Position = Heading; //Logger.Trace("Ticking with no path steps left"); 
                        this.Done = true; }

            },
            (tick) =>
            {
                this.Owner.Position = Heading;
                //Logger.Trace("Completed! Path contains :" + this.Path.Count);
                this.Done = true;
            });

            this.Started = true;
        }

        public override void Update(int tickCounter)
        {
            this.Timer.Update(tickCounter);
        }

        public override void Cancel(int tickCounter)
        {
            this.Done = true;
        }
    }
}
