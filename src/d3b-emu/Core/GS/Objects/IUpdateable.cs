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

namespace D3BEmu.Core.GS.Objects
{
    /// <summary>
    /// Defines an interface for objects that can update on game ticks.
    /// </summary>
    public interface IUpdateable
    {
        /// <summary>
        /// Tells object to update itself and call it's IUpdateable childs if any.
        /// </summary>
        /// <param name="tickCounter">The Game.TickCounter value when the function gets called.</param>
        void Update(int tickCounter);
    }
}
