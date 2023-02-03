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
    class AncientTalic : Minion
    {
        public AncientTalic(Map.World world, PowerContext context, int AncientsID)
            : base(world, 90535, context.User, null)
        {
            Scale = 1.2f; //they look cooler bigger :)
            //TODO: get a proper value for this.
            this.WalkSpeed *= 5;
            SetBrain(new MinionBrain(this));
            (Brain as MinionBrain).AddPresetPower(30592);  //Weapon_Instant
            (Brain as MinionBrain).AddPresetPower(187092); //basic melee
            (Brain as MinionBrain).AddPresetPower(168825); //Leap //Only Active with Rune_E
            (Brain as MinionBrain).AddPresetPower(168830); //WhirlWind
            //TODO: These values should most likely scale, but we don't know how yet, so just temporary values.
            Attributes[GameAttribute.Hitpoints_Max] = 20f;
            Attributes[GameAttribute.Hitpoints_Cur] = 20f;
            Attributes[GameAttribute.Attacks_Per_Second] = 1.0f;

            Attributes[GameAttribute.Damage_Weapon_Min, 0] = context.ScriptFormula(11) * context.User.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0];
            Attributes[GameAttribute.Damage_Weapon_Delta, 0] = context.ScriptFormula(13) * context.User.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0];

            Attributes[GameAttribute.Pet_Type] = 0x8;
            //Pet_Owner and Pet_Creator seems to be 0
            (context.User as Player).InGameClient.SendMessage(new PetMessage()
            {
                Field0 = 0,
                Field1 = AncientsID,
                PetId = this.DynamicID,
                Field3 = 0x8,
            });
        }
    }
}
