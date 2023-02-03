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
using D3BEmu.Core.GS.Skills;
using D3BEmu.Core.GS.Ticker;
using D3BEmu.Core.GS.Actors;
using D3BEmu.Net.GS.Message;


namespace D3BEmu.Core.GS.Powers.Implementations
{
    [ImplementsPowerSNO(30592)]  // Weapon_Melee_Instant.pow
    public class WeaponMeleeInstant : ActionTimedSkill
    {
        public override IEnumerable<TickTimer> Main()
        {
            WeaponDamage(GetBestMeleeEnemy(), 1.00f, DamageType.Physical);
            yield break;
        }

        public override float GetActionSpeed()
        {
            // for some reason the formula for _Instant.pow does not multiply by 1.1 even though it should
            // manually scale melee speed
            return base.GetActionSpeed() * 1.1f;
        }
    }
}
