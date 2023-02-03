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
using D3BEmu.Core.GS.Common.Types.Math;

namespace D3BEmu.Net.GS.Message.Fields
{
    public class WorldLocationMessageData
    {
        public float Scale;
        public PRTransform Transform;
        public uint WorldID;

        public void Parse(GameBitBuffer buffer)
        {
            Scale = buffer.ReadFloat32();
            Transform = new PRTransform();
            Transform.Parse(buffer);
            WorldID = buffer.ReadUInt(32);
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteFloat32(Scale);
            Transform.Encode(buffer);
            buffer.WriteUInt(32, WorldID);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("WorldLocationMessageData:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("Scale: " + Scale.ToString("G"));
            Transform.AsText(b, pad);
            b.Append(' ', pad);
            b.AppendLine("WorldID: 0x" + WorldID.ToString("X8") + " (" + WorldID + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
