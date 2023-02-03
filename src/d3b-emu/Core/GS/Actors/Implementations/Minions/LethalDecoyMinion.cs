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
    class LethalDecoyMinion : Minion
    {
        public LethalDecoyMinion(Map.World world, PowerContext context, int ImageID)
            : base(world, 99241, context.User, null)    // male monk decoy
        {
            Scale = 1.2f; //they look !cooler bigger :)
            //TODO: get a proper value for this.
            this.WalkSpeed *= 5;
            SetBrain(new MinionBrain(this));
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
                Field1 = ImageID,
                PetId = this.DynamicID,
                Field3 = 0x8,
            });

            #region SNOs
            /* 
            Actor
            [143429] combatStaff_lethaldecoy_crimson_damage
            [143447] Sword_lethaldecoy_golden_spiritLeech
            [143466] Polearm_lethaldecoy_alabaster_guardian
            [147208] Monk_LethalDecoy_alabaster_deserter
            [098940] Monk_LethalDecoy
            [099241] Monk_Male_lethalDecoy
            [208435] Monk_Female_lethalDecoy
            [208441] Monk_Female_lethalDecoy_alabaster_holyGuardian
            [208443] Monk_Female_lethalDecoy_crimson_damage
            [208445] Monk_Female_lethalDecoy_golden_spiritLeech
            [208451] Monk_Female_LethalDecoy_obsidian_fighter
            [101550] Monk_lethalDecoy_glow
            [142477] Monk_LethalDecoy_crimson_damage
            [142478] Monk_LethalDecoy_obsidian_fighter
            [142479] Monk_LethalDecoy_golden_stealSpirit
            [142481] Monk_Male_lethalDecoy_crimson_damage
            [142503] Monk_Male_lethalDecoy_golden_spiritLeech
            [142514] Monk_lethalDecoy_glow_golden_spiritLeech
            [142826] Monk_lethalDecoy_glow_alabaster_guardian
            [142862] Monk_Male_lethalDecoy_alabaster_holyGuardian

            EG
            [099504] Monk_lethalDecoy_explosion - 0
            [142482] Monk_lethalDecoy_explosion_crimson_damage - 0
            [142483] Monk_lethalDecoy_explosion_silver - 0
            [142484] Monk_lethalDecoy_explosion_bronze - 0
            */
            #endregion
        }
    }
}