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

using System.Text;
using CrystalMpq;
using Gibbed.IO;
using D3BEmu.Net.GS.Message;

namespace D3BEmu.Core.GS.Common.Types.Math
{
    public class Vector2D
    {
        public int X;
        public int Y;

        public Vector2D() { }

        /// <summary>
        /// Reads Vector2D from given MPQFileStream.
        /// </summary>
        /// <param name="stream">The MPQFileStream to read from.</param>
        public Vector2D(MpqFileStream stream)
        {
            X = stream.ReadValueS32();
            Y = stream.ReadValueS32();
        }

        public Vector2D(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Parses Vector2D from given GameBitBuffer.
        /// </summary>
        /// <param name="buffer">The GameBitBuffer to parse from.</param>
        public void Parse(GameBitBuffer buffer)
        {
            X = buffer.ReadInt(32);
            Y = buffer.ReadInt(32);
        }

        /// <summary>
        /// Encodes Vector2D to given GameBitBuffer.
        /// </summary>        
        /// <param name="buffer">The GameBitBuffer to write.</param>
        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, X);
            buffer.WriteInt(32, Y);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("Vector2D:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("X: 0x" + X.ToString("X8") + " (" + X + ")");
            b.Append(' ', pad);
            b.AppendLine("Y: 0x" + Y.ToString("X8") + " (" + Y + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }

        public override string ToString()
        {
            return string.Format("x:{0} y:{1}", X, Y);
        }
    }
}
