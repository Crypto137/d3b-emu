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
    public class EntityId
    {
        public long High;
        public long Low;

        public void Parse(GameBitBuffer buffer)
        {
            High = buffer.ReadInt64(64);
            Low = buffer.ReadInt64(64);
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt64(64, High);
            buffer.WriteInt64(64, Low);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("EntityId:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("Field0: 0x" + High.ToString("X16"));
            b.Append(' ', pad);
            b.AppendLine("Field1: 0x" + Low.ToString("X16"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}