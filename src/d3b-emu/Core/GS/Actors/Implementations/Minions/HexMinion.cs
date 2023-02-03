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
    class HexMinion : Minion
    {
        public HexMinion(Map.World world, PowerContext context, int HexID)
            : base(world, 107826, context.User, null)
        {
            Scale = 1f;
            //TODO: get a proper value for this.
            this.WalkSpeed *= 5;
            SetBrain(new MinionBrain(this));
            (Brain as MinionBrain).AddPresetPower(196974); //chicken_walk.pow
            (Brain as MinionBrain).AddPresetPower(188442); //explode.pow
            (Brain as MinionBrain).AddPresetPower(107301); //Fetish.pow
            (Brain as MinionBrain).AddPresetPower(107742); //Heal

            Attributes[GameAttribute.Hitpoints_Max] = 5f;
            Attributes[GameAttribute.Hitpoints_Cur] = 5f;
            Attributes[GameAttribute.Attacks_Per_Second] = 1.0f;

            Attributes[GameAttribute.Damage_Weapon_Min, 0] = 5f;
            Attributes[GameAttribute.Damage_Weapon_Delta, 0] = 7f;

            Attributes[GameAttribute.Pet_Type] = 0x8;
            //Pet_Owner and Pet_Creator seems to be 0
            (context.User as Player).InGameClient.SendMessage(new PetMessage()
            {
                Field0 = 0,
                Field1 = HexID,
                PetId = this.DynamicID,
                Field3 = 0x8,
            });
        }
    }
}
