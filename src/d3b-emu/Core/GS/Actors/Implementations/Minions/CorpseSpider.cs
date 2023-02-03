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
using D3BEmu.Core.GS.Powers;
using D3BEmu.Core.GS.AI.Brains;
using D3BEmu.Net.GS.Message;
using D3BEmu.Core.GS.Ticker;
using D3BEmu.Core.GS.Players;
using D3BEmu.Net.GS.Message.Definitions.Pet;

namespace D3BEmu.Core.GS.Actors.Implementations.Minions
{
    class CorpseSpider : Minion
    {

        //107031, 106731, 106749, 107067, 107107, 107112

        public CorpseSpider(Map.World world, PowerContext context, int SpiderID)
            : base(world, 107031, context.User, null)
        {
            Scale = 1.2f; //they look cooler bigger :)
            //TODO: get a proper value for this.
            this.WalkSpeed *= 5;
            SetBrain(new MinionBrain(this));
            (Brain as MinionBrain).AddPresetPower(30592); //melee_instant
            (Brain as MinionBrain).AddPresetPower(30005); //AINearby
            (Brain as MinionBrain).AddPresetPower(107103); //Spider_leap
            //TODO: These values should most likely scale, but we don't know how yet, so just temporary values.
            Attributes[GameAttribute.Hitpoints_Max] = 20f;
            Attributes[GameAttribute.Hitpoints_Cur] = 20f;
            Attributes[GameAttribute.Attacks_Per_Second] = 1.0f;

            Attributes[GameAttribute.Damage_Weapon_Min, 0] = context.ScriptFormula(16) * context.User.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0];
            Attributes[GameAttribute.Damage_Weapon_Delta, 0] = context.ScriptFormula(17) * context.User.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0];

            Attributes[GameAttribute.Pet_Type] = 0x8;
            //Pet_Owner and Pet_Creator seems to be 0
            (context.User as Player).InGameClient.SendMessage(new PetMessage()
            {
                Field0 = 0,
                Field1 = SpiderID,
                PetId = this.DynamicID,
                Field3 = 0x8,
            });
        }
    }
}
