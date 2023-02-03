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
using D3BEmu.Common.MPQ.FileFormats;
using D3BEmu.Net.GS.Message;

namespace D3BEmu.Core.GS.Items.Implementations
{
    // A quick example of Type handling, HealthPotion will always override Potion (for Health potions ofc ;))!
    // fasbat
    [HandledType("HealthPotion")]
    public class HealthPotion : Potion
    {
        public HealthPotion(Map.World world, ItemTable definition)
            : base(world, definition)
        {
        }

        public override void OnRequestUse(GS.Players.Player player, Item target, int actionId, Net.GS.Message.Fields.WorldPlace worldPlace)
        {
            if (player.Attributes[GameAttribute.Hitpoints_Cur] == player.Attributes[GameAttribute.Hitpoints_Max])
                return; // TODO Error msg? /fasbat
            player.Attributes[GameAttribute.Hitpoints_Cur] =
                Math.Min(player.Attributes[GameAttribute.Hitpoints_Cur] + this.Attributes[GameAttribute.Hitpoints_Granted],
                player.Attributes[GameAttribute.Hitpoints_Max]);

            player.Attributes.BroadcastChangedIfRevealed();

            if (this.Attributes[GameAttribute.ItemStackQuantityLo] <= 1)
                player.Inventory.DestroyInventoryItem(this); // No more potions!
            else
            {
                this.Attributes[GameAttribute.ItemStackQuantityLo]--; // Just remove one
                this.Attributes.SendChangedMessage(player.InGameClient);
            }

        }
    }
}