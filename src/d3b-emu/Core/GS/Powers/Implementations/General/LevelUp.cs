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
using D3BEmu.Core.GS.Ticker;
using D3BEmu.Net.GS.Message;
using D3BEmu.Core.GS.Players;

namespace D3BEmu.Core.GS.Powers.Implementations
{
    [ImplementsPowerSNO(85954)] //g_levelup.pow
    public class LevelUpBlast : PowerScript
    {
        public override IEnumerable<TickTimer> Run()
        {
            User.PlayEffectGroup(Player.LevelUpEffects[this.User.Attributes[GameAttribute.Level]]);
            yield return WaitSeconds(0.6f);
            WeaponDamage(GetEnemiesInRadius(User.Position, ScriptFormula(2)), ScriptFormula(0), DamageType.Physical);
            yield break;
        }
    }
}