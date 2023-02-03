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

namespace D3BEmu.Core.EmuNet.Helpers
{
    public static class EntityIdHelper
    {
        /// <summary>
        /// Returns high-id type for given bnet.protocol.EntityId
        /// </summary>
        /// <param name="id">The bnet.protocol.EntityId</param>
        /// <returns><see cref="HighIdType"/></returns>
        public static HighIdType GetHighIdType(this bnet.protocol.EntityId id)
        {
            switch (id.High)
            {
                case 0x100000000000000:
                    return HighIdType.AccountId;
                case 0x200000000000000:
                    return HighIdType.GameAccountId;
                case 0x300000000000000:
                    return HighIdType.ToonId;
                case 0x400000000000000:
                    return HighIdType.GameId;
                case 0x600000000000000:
                    return HighIdType.ChannelId;
            }
            return HighIdType.Unknown;
        }

        /// <summary>
        /// High id types for bnet.protocol.EntityId high-id.
        /// </summary>
        public enum HighIdType : ulong
        {
            Unknown = 0x0,
            AccountId = 0x100000000000000,
            GameAccountId = 0x200000000000000,
            ToonId = 0x300000000000000,
            GameId = 0x400000000000000,
            ChannelId = 0x600000000000000
        }
    }
}
