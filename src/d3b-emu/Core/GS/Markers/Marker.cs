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

namespace D3BEmu.Core.GS.Markers
{
    public enum MarkerTypes : int
    {
        Checkpoint = 3795,
        /// <summary>
        /// Single Player start location
        /// </summary>
        Start_Location_0 = 5502,
        /// <summary>
        /// Multiplayer start location
        /// </summary>
        Start_Location_Team_0= 5503,
        Waypoint = 6442,
        Banner_Player_1 = 123714,
        Banner_Player_2 = 123715,
        Banner_Player_3 = 123716,
        Banner_Player_4 = 123717,
        Savepoint = 180941,
    }
}
