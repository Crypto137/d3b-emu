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

namespace D3BEmu.Core.GS.Objects
{
    /// <summary>
    /// A dynamic object that can have a dynamicId
    /// </summary>
    public abstract class DynamicObject
    {
        /// <summary>
        /// The dynamic unique runtime ID for the actor.
        /// </summary>
        public readonly uint DynamicID;

        /// <summary>
        /// Initialization constructor.
        /// </summary>
        /// <param name="dynamicID">The dynamic ID to initialize with.</param>
        protected DynamicObject(uint dynamicID)
        {
            this.DynamicID = dynamicID;
        }

        /// <summary>
        /// Destroy the object. This should remove any references to the object throughout GS.
        /// </summary>
        public abstract void Destroy();
    }
}
