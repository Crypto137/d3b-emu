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
using System.Text;
using D3BEmu.Core.GS.Common.Types.Math;
using D3BEmu.Net.GS.Message.Definitions.ACD;
using D3BEmu.Core.GS.Common.Types.TagMap;
using D3BEmu.Core.GS.Ticker;

namespace D3BEmu.Core.GS.Powers.Implementations
{
    [ImplementsPowerSNO(70432)]  // Knockback.pow
    public class KnockbackBuff : Buff
    {
        public TickTimer ArrivalTime { get { return _mover.ArrivalTime; } }

        private float _magnitude;
        private float _height;
        private float _gravity;
        private ActorMover _mover;

        public KnockbackBuff(float magnitude, float arcHeight = 3.0f, float arcGravity = -0.03f)
        {
            _magnitude = magnitude;
            _height = arcHeight;
            _gravity = arcGravity;
        }

        public override bool Apply()
        {
            if (!base.Apply())
                return false;

            Vector3D destination = PowerMath.TranslateDirection2D(User.Position, Target.Position,
                                                                   _magnitude < 0f ? User.Position : Target.Position, 
                                                                   (float)Math.Sqrt(Math.Abs(_magnitude)));

            _mover = new ActorMover(Target);
            _mover.MoveArc(destination, _height, _gravity, new ACDTranslateArcMessage
            {
                Field3 = 0x2006, // wtf?
                FlyingAnimationTagID = AnimationSetKeys.KnockBack.ID,
                LandingAnimationTagID = AnimationSetKeys.KnockBackLand.ID,
                PowerSNO = PowerSNO
            });

            return true;
        }

        public override bool Update()
        {
            return _mover.Update();
        }

        public override bool Stack(Buff buff)
        {
            // not sure how knockbacks would be combined, so just swallow all knockback stacks for now
            // updated stacked buff with mover so arrival time can be read for would-be-stacked buff.
            ((KnockbackBuff)buff)._mover = _mover;
            return true;
        }
    }
}
