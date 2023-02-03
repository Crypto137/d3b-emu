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
using System.Numerics;
using System.Text;

namespace D3BEmu.Common.Extensions
{
    public static class BigIntegerExtensions
    {
        public static BigInteger ToBigInteger(this byte[] src)
        {
            var dst = new byte[src.Length + 1];
            Array.Copy(src, dst, src.Length);
            return new BigInteger(dst);
        }

        public static byte[] ToArray(this BigInteger b)
        {
            var result = b.ToByteArray();
            if (result[result.Length - 1] == 0 && (result.Length % 0x10) != 0)
                Array.Resize(ref result, result.Length - 1);
            return result;
        }
    }
}
