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
using D3BEmu.Core.GS.Common.Types.Math;
using D3BEmu.Core.GS.Objects;
using D3BEmu.Core.GS.Map;
using D3BEmu.Core.GS.Actors;
using D3BEmu.Core.GS.Ticker;
using D3BEmu.Net.GS.Message;

namespace D3BEmu.Core.GS.Powers
{
    public class EffectActor : Actor, IUpdateable
    {
        public PowerContext Context;

        public TickTimer Timeout = null;
        public float UpdateDelay = 0f;
        public Action OnUpdate = null;
        public Action OnTimeout = null;

        public override ActorType ActorType { get { return Actors.ActorType.ClientEffect; } }

        private TickTimer _updateTimer;

        public EffectActor(PowerContext context, int actorSNO, Vector3D position)
            : base(context.World, actorSNO)
        {
            this.Context = context;

            this.Field2 = 0x8;
            if (this.Scale == 0f)
                this.Scale = 1f;
            this.Position = position;

            // copy in important effect params from user
            this.Attributes[GameAttribute.Rune_A, context.PowerSNO] = context.User.Attributes[GameAttribute.Rune_A, context.PowerSNO];
            this.Attributes[GameAttribute.Rune_B, context.PowerSNO] = context.User.Attributes[GameAttribute.Rune_B, context.PowerSNO];
            this.Attributes[GameAttribute.Rune_C, context.PowerSNO] = context.User.Attributes[GameAttribute.Rune_C, context.PowerSNO];
            this.Attributes[GameAttribute.Rune_D, context.PowerSNO] = context.User.Attributes[GameAttribute.Rune_D, context.PowerSNO];
            this.Attributes[GameAttribute.Rune_E, context.PowerSNO] = context.User.Attributes[GameAttribute.Rune_E, context.PowerSNO];
        }

        public void Spawn(float facingAngle = 0)
        {
            this.SetFacingRotation(facingAngle);
            this.World.Enter(this);
        }

        public virtual void Update(int tickCounter)
        {
            if (Timeout != null && Timeout.TimedOut)
            {
                if (OnTimeout != null)
                    OnTimeout();

                this.Destroy();
            }
            else if (OnUpdate != null)
            {
                if (_updateTimer == null || _updateTimer.TimedOut)
                {
                    OnUpdate();
                    if (this.UpdateDelay > 0f)
                        _updateTimer = new SecondsTickTimer(this.Context.World.Game, this.UpdateDelay);
                    else
                        _updateTimer = null;
                }
            }
        }
    }
}
