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

namespace D3BEmu.Net.GS.Message.Definitions.Connection
{
    [Message(Opcodes.LogoutTickTimeMessage)]
    public class LogoutTickTimeMessage : GameMessage
    {
        public bool Field0;
        public int Ticks;
        public int Field2;

        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadBool();
            Ticks = buffer.ReadInt(32);
            Field2 = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteBool(Field0);
            buffer.WriteInt(32, Ticks);
            buffer.WriteInt(32, Field2);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("LogoutTickTimeMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: " + (Field0 ? "true" : "false"));
            b.Append(' ', pad); b.AppendLine("Ticks: 0x" + Ticks.ToString("X8") + " (" + Ticks + ")");
            b.Append(' ', pad); b.AppendLine("Field2: 0x" + Field2.ToString("X8") + " (" + Field2 + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}