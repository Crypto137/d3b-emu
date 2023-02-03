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

namespace D3BEmu.Core.GS.AI
{
    /// <summary>
    /// Available brain states.
    /// </summary>
    public enum BrainState
    {
        /// <summary>
        /// The idle state, which basically means brain never got an update.
        /// </summary>
        Idle,

        /// <summary>
        /// The wandering state.
        /// </summary>
        Wander,

        /// <summary>
        /// Attack nearby enemies.
        /// </summary>
        Combat,

        /// <summary>
        /// Follow.
        /// </summary>
        Follow,

        /// <summary>
        /// Follow and guard.
        /// </summary>
        Guard,

        /// <summary>
        /// I see dead brains.
        /// </summary>
        Dead,
        Off,
        End
    }
}
