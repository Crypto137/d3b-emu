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

namespace D3BEmu.Net.GS.Message.Fields
{
    public class AnimPreplayData
    {
        public int Field0;
        public int Field1;   // This seems to constantly increase trough a game - farmy
        public int Field2;

        public void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadInt(32);
            Field1 = buffer.ReadInt(32);
            Field2 = buffer.ReadInt(32);
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, Field0);
            buffer.WriteInt(32, Field1);
            buffer.WriteInt(32, Field2);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("AnimPreplayData:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("Field0: 0x" + Field0.ToString("X8") + " (" + Field0 + ")");
            b.Append(' ', pad);
            b.AppendLine("Field1: 0x" + Field1.ToString("X8") + " (" + Field1 + ")");
            b.Append(' ', pad);
            b.AppendLine("Field2: 0x" + Field2.ToString("X8") + " (" + Field2 + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}