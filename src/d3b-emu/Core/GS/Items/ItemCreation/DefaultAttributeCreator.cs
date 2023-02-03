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

using D3BEmu.Common.Helpers.Math;
using D3BEmu.Net.GS.Message;
using D3BEmu.Common.Helpers;

namespace D3BEmu.Core.GS.Items.ItemCreation
{
    class DefaultAttributeCreator : IItemAttributeCreator
    {
        public void CreateAttributes(Item item)
        {
            item.Attributes[GameAttribute.Item_Quality_Level] = 1;
            item.Attributes[GameAttribute.Seed] = RandomHelper.Next(); //unchecked((int)2286800181);
        }
    }
}
