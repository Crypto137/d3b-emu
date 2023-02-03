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
using System.Linq;
using System.Text;
using D3BEmu.Core.GS.Ticker;
using D3BEmu.Core.GS.Common.Types.TagMap;
using D3BEmu.Core.GS.Players;
using D3BEmu.Net.GS.Message.Definitions.Animation;
using D3BEmu.Net.GS.Message.Fields;

namespace D3BEmu.Core.GS.Powers
{
    public abstract class Skill : PowerScript
    {
        // main handler called to generate effects for skill
        public abstract IEnumerable<TickTimer> Main();

        public sealed override IEnumerable<TickTimer> Run()
        {
            // play starting animation and effects
            _PlayActionAnimation();
            _PlayCastEffect();

            float contactDelay = GetContactDelay();
            if (contactDelay > 0f)
                yield return WaitSeconds(contactDelay);

            _PlayContactEffect();

            // run main effects script
            foreach (TickTimer timer in Main())
                yield return timer;
        }

        public bool IsUserFemale
        {
            get
            {
                return User is Player && (User as Player).Toon.Gender == 2;  // 2 = female
            }
        }

        public virtual int GetActionAnimationSNO()
        {
            int tag = EvalTag(PowerKeys.AnimationTag);
            if (User.AnimationSet != null && User.AnimationSet.Animations.ContainsKey(tag))
                return User.AnimationSet.Animations[tag];
            else
                return -1;
        }

        public virtual float GetActionSpeed()
        {
            return EvalTag(PowerKeys.AttackSpeed);
        }

        public virtual int GetCastEffectSNO()
        {
            return EvalTag(IsUserFemale ? PowerKeys.CastingEffectGroup_Female : PowerKeys.CastingEffectGroup_Male);
        }

        public virtual int GetContactEffectSNO()
        {
            return EvalTag(IsUserFemale ? PowerKeys.ContactFrameEffectGroup_Female : PowerKeys.ContactFrameEffectGroup_Male);
        }

        public virtual float GetContactDelay()
        {
            return 0f;
        }

        private void _PlayActionAnimation()
        {
            int animationSNO = GetActionAnimationSNO();
            float speed = GetActionSpeed();
            if (animationSNO != -1 && speed > 0f)
            {
                PlayAnimationMessage message = new PlayAnimationMessage
                {
                    ActorID = User.DynamicID,
                    Field1 = 3,
                    Field2 = 0,
                    tAnim = new PlayAnimationMessageSpec[]
                    {
                        new PlayAnimationMessageSpec
                        {
                            Duration = (int)(60f / speed),  // ticks
                            AnimationSNO = animationSNO,
                            PermutationIndex = 0x0,
                            Speed = speed,
                        }
                    }
                };

                if (User is Player)
                    User.World.BroadcastExclusive(message, User);
                else
                    User.World.BroadcastIfRevealed(message, User);
            }
        }

        private void _PlayCastEffect()
        {
            int sno = GetCastEffectSNO();
            if (sno != -1)
                User.PlayEffectGroup(sno);
        }

        private void _PlayContactEffect()
        {
            int sno = GetContactEffectSNO();
            if (sno != -1)
                User.PlayEffectGroup(sno);
        }
    }
}
