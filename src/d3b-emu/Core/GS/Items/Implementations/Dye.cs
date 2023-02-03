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

using System.Collections.Generic;
using System.Diagnostics;
using D3BEmu.Net.GS.Message;
using D3BEmu.Core.GS.Players;
using D3BEmu.Net.GS.Message.Fields;
using D3BEmu.Core.GS.Map;

namespace D3BEmu.Core.GS.Items.Implementations
{
    // This is how Dyes should be implemented ;) /fasbat
    [HandledType("Dye")]
    public class Dye : Item
    {
        private static Dictionary<int, int> DyeColorMap = new Dictionary<int,int>();

        public Dye(World world, D3BEmu.Common.MPQ.FileFormats.ItemTable definition)
            : base(world, definition)
        {
        }

        public override void OnRequestUse(Player player, Item target, int actionId, WorldPlace worldPlace)
        {
            Debug.Assert(target != null);

            target.Attributes[GameAttribute.DyeType] = this.Attributes[GameAttribute.DyeType];
            player.Inventory.DestroyInventoryItem(this);
            player.Inventory.SendVisualInventory(player); // TODO: Send it to all who see! /fasbat
        }
    }
}
