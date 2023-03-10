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
using D3BEmu.Net.GS.Message;

namespace D3BEmu.Core.GS.Common.Types.Math
{
    public class PRTransform
    {
        public Quaternion Quaternion;
        public Vector3D Vector3D;

        public PRTransform() { }

        /// <summary>
        /// Reads PRTransform from given MPQFileStream.
        /// </summary>
        /// <param name="stream">The MPQFileStream to read from.</param>
        public PRTransform(MpqFileStream stream)
        {
            Quaternion = new Quaternion(stream);
            Vector3D = new Vector3D(stream);
        }

        /// <summary>
        /// Reads PRTransform from given GameBitBuffer.
        /// </summary>
        /// <param name="buffer">The GameBitBuffer to parse from.</param>
        public void Parse(GameBitBuffer buffer)
        {
            Quaternion = new Quaternion();
            Quaternion.Parse(buffer);
            Vector3D = new Vector3D();
            Vector3D.Parse(buffer);
        }

        /// <summary>
        /// Encodes PRTransform to given GameBitBuffer.
        /// </summary>
        /// <param name="buffer">The GameBitBuffer to write.</param>
        public void Encode(GameBitBuffer buffer)
        {
            Quaternion.Encode(buffer);
            Vector3D.Encode(buffer);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PRTransform:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            Quaternion.AsText(b, pad);
            Vector3D.AsText(b, pad);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
