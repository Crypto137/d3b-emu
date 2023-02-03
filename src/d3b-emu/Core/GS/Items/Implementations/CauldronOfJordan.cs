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
using System.Text;

namespace D3BEmu.Core.GS.Items.Implementations
{
    [HandledItem("StoneOfWealth")]
    class CauldronOfJordan : Item
    {
        public CauldronOfJordan(GS.Map.World world, D3BEmu.Common.MPQ.FileFormats.ItemTable definition)
            : base(world, definition)
        {
        }

        public override void OnTargeted(Players.Player player, Net.GS.Message.Definitions.World.TargetMessage message)
        {
            player.EnableCauldronOfJordan();
            this.Destroy();
        }

        public static void OnUse(GS.Players.Player player, Item sellItem)
        {
            int sellValue = sellItem.ItemDefinition.BaseGoldValue; // TODO: calculate correct sell value for magic items
            player.Inventory.AddGoldAmount(sellValue);

            // TODO: instead of destroying item, it should be moved to merchants inventory for rebuy. 
            player.Inventory.DestroyInventoryItem(sellItem);
        }
    }
}