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

using System.Linq;
using System.Text;

namespace D3BEmu.Common.Helpers.Hash
{
    public class StringHashHelper
    {
        // Used on the full name of a proto service for their ServiceHash and for toon names
        // This algorithm is FNV-1a with prime 0x1000193 /komiga
        public static uint HashIdentity(string input)
        {
            var bytes = Encoding.ASCII.GetBytes(input);
            return bytes.Aggregate(0x811C9DC5, (current, t) => 0x1000193 * (t ^ current));
        }

        // Hash algorithm used for item names
        // FIXME: Our item name hasher seems to be problematic, for some items even with valid snoId's, bad gbId's are hashed which crashes client on pickup. /raist.
        // NOTE: This is the DJB algorithm with a base of 0; and is likely accurate, assuming the base is actually 0 (the normal DJB uses 5381 as the base).
        //       Someone mentioned that the issues are due to not all assets being available in the beta for some items. /komiga
        public static int HashItemName(string input)
        {
            int hash = 0;
            input = input.ToLower();
            for (int i = 0; i < input.Length; ++i)
                hash = (hash << 5) + hash + input[i];
            return hash;
        }

        /// <summary>
        /// Hashes a string to an int. This hashing is CASE SENSITIVE
        /// </summary>
        public static int HashNormal(string input)
        {
            int hash = 0;
            for (int i = 0; i < input.Length; ++i)
                hash = (hash << 5) + hash + input[i];
            return hash;
        }

    }
}
