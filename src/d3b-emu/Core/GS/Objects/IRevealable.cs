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

using D3BEmu.Core.GS.Players;

namespace D3BEmu.Core.GS.Objects
{
    /// <summary>
    /// Interface for revealable in-game objects.
    /// </summary>
    public interface IRevealable
    {
        /// <summary>
        /// Reveals the object to a player.
        /// </summary>
        /// <returns>true if the object was revealed or false if the object was already revealed.</returns>
        bool Reveal(Player player);

        /// <summary>
        /// Unreveals the object from a player.
        /// </summary>
        /// <returns>true if the object was unrevealed or false if the object wasn't already revealed.</returns>
        bool Unreveal(Player player);
    }
}
