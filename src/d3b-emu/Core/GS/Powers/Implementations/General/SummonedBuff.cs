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

using D3BEmu.Net.GS.Message;
using D3BEmu.Common.MPQ.FileFormats;

namespace D3BEmu.Core.GS.Powers.Implementations
{
    [ImplementsPowerSNO(30540)]
    public class SummonedBuff : TimedBuff
    {
        public override void Init()
        {
            base.Init();
            Timeout = WaitSeconds(3f);  // TODO: calculate this based on spawn animation length
        }

        public override bool Apply()
        {
            base.Apply();

            // lookup and play spawn animation, otherwise fail
            if (this.Target.AnimationSet != null && this.Target.AnimationSet.TagExists(AnimationTags.Spawn))
            {
                this.Target.PlayActionAnimation(this.Target.AnimationSet.GetAniSNO(AnimationTags.Spawn));
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
