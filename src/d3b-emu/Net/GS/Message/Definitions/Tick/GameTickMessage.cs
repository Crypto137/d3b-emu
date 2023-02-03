﻿/*
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

namespace D3BEmu.Net.GS.Message.Definitions.Tick
{
    [Message(Opcodes.GameTickMessage)]
    public class GameTickMessage:GameMessage
    {
        public int Tick;

        public GameTickMessage() : base(Opcodes.GameTickMessage) { }

        public GameTickMessage(int tick)
            : base(Opcodes.GameTickMessage)
        {
            this.Tick = tick;
        }

        public override void Parse(GameBitBuffer buffer)
        {
            Tick = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, Tick);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("DWordDataMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: 0x" + Tick.ToString("X8") + " (" + Tick + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
