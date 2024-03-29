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

using D3BEmu.Core.GS.Actors.Actions;
using D3BEmu.Core.GS.Common.Types.Math;
using D3BEmu.Core.GS.Ticker;

namespace D3BEmu.Core.GS.Actors.Movement
{
    public class MoveToPointAction : ActorAction
    {
        public Vector3D Heading { get; private set; }

        public SteppedRelativeTickTimer Timer;

        public MoveToPointAction(Actor owner, Vector3D heading)
            : base(owner)
        {
            this.Heading = heading;
        }

        public override void Start(int tickCounter)
        {
            var distance = MovementHelpers.GetDistance(this.Owner.Position, this.Heading);
            var facingAngle = MovementHelpers.GetFacingAngle(this.Owner, this.Heading);
            this.Owner.Move(this.Heading, facingAngle);

            Logger.Trace("Heading: " + this.Heading);
            Logger.Trace("Start point: " + this.Owner.Position);

            this.Timer = new SteppedRelativeTickTimer(this.Owner.World.Game, 6, (int)(distance / this.Owner.WalkSpeed),
            (tick) =>
            {
                this.Owner.Position = MovementHelpers.GetMovementPosition(this.Owner.Position, this.Owner.WalkSpeed, facingAngle, 6);
                Logger.Trace("Step: " + this.Owner.Position);
            },
            (tick) =>
            {
                this.Owner.Position = Heading;
                Logger.Trace("Completed: " + this.Owner.Position);
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
