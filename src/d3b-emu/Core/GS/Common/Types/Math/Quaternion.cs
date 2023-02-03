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
    public class Quaternion
    {
        public float W;
        public Vector3D Vector3D;

        public Quaternion() { }

        /// <summary>
        /// Creates an quaternion that rotates along the Z-axis by the specified "facing" angle. 
        /// </summary>
        /// <param name="facingAngle">The angle in radians.</param>
        /// <returns></returns>
        public static Quaternion FacingRotation(float facingAngle)
        {
            return new Quaternion
            {
                W = (float)System.Math.Cos(facingAngle / 2f),
                Vector3D = new Vector3D(0, 0, (float)System.Math.Sin(facingAngle / 2f))
            };
        }

        /// <summary>
        /// Reads Quaternion from given MPQFileStream.
        /// </summary>
        /// <param name="stream">The MPQFileStream to read from.</param>
        public Quaternion(MpqFileStream stream)
        {
            this.Vector3D = new Vector3D(stream.ReadValueF32(), stream.ReadValueF32(), stream.ReadValueF32());
            this.W = stream.ReadValueF32();
        }

        /// <summary>
        /// Parses Quaternion from given GameBitBuffer.
        /// </summary>
        /// <param name="buffer">The GameBitBuffer to parse from.</param>
        public void Parse(GameBitBuffer buffer)
        {
            W = buffer.ReadFloat32();
            Vector3D = new Vector3D();
            Vector3D.Parse(buffer);
        }

        /// <summary>
        /// Encodes Quaternion to given GameBitBuffer.
        /// </summary>
        /// <param name="buffer">The GameBitBuffer to write.</param>
        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteFloat32(W);
            Vector3D.Encode(buffer);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("Quaternion:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("W: " + W.ToString("G"));
            Vector3D.AsText(b, pad);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}