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
using D3BEmu.Core.GS.Common.Types.Math;
using D3BEmu.Net.GS.Message;

namespace D3BEmu.Core.GS.Common.Types.Collision
{
    public class AABB
    {
        public Vector3D Min;
        public Vector3D Max;

        public AABB() { }

        /// <summary>
        /// Reads AABB from given MPQFileStream.
        /// </summary>
        /// <param name="stream">The MPQFileStream to read from.</param>
        public AABB(MpqFileStream stream)
        {
            this.Min = new Vector3D(stream.ReadValueF32(), stream.ReadValueF32(), stream.ReadValueF32());
            this.Max = new Vector3D(stream.ReadValueF32(), stream.ReadValueF32(), stream.ReadValueF32());
        }

        /// <summary>
        /// Parses AABB from given GameBitBuffer.
        /// </summary>
        /// <param name="buffer">The GameBitBuffer to parse from.</param>
        public void Parse(GameBitBuffer buffer)
        {
            Min = new Vector3D();
            Min.Parse(buffer);
            Max = new Vector3D();
            Max.Parse(buffer);
        }

        /// <summary>
        /// Encodes AABB to given GameBitBuffer.
        /// </summary>
        /// <param name="buffer">The GameBitBuffer to write.</param>
        public void Encode(GameBitBuffer buffer)
        {
            Min.Encode(buffer);
            Max.Encode(buffer);
        }

        public bool IsWithin(Vector3D v)
        {
            if (v >= this.Min &&
                v <= this.Max)
            {
                return true;
            }
            return false;
        }

        public bool Intersects(AABB other)
        {
            if (// Max < o.Min
                this.Max.X < other.Min.X ||
                this.Max.Y < other.Min.Y ||
                this.Max.Z < other.Min.Z ||
                // Min > o.Max
                this.Min.X > other.Max.X ||
                this.Min.Y > other.Max.Y ||
                this.Min.Z > other.Max.Z)
            {
                return false;
            }
            return true; // Intersects if above fails
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("AABB:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            Min.AsText(b, pad);
            Max.AsText(b, pad);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }

        public override string ToString()
        {
            return string.Format("AABB: min:{0} max:{1}", this.Min, this.Max);
        }
    }
}
