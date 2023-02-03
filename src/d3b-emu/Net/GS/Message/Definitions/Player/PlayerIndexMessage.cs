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

namespace D3BEmu.Net.GS.Message.Definitions.Player
{
    [Message(new[] {Opcodes.PlayerIndexMessage1, Opcodes.PlayerIndexMessage2, Opcodes.PlayerIndexMessage3, Opcodes.PlayerIndexMessage4, Opcodes.PlayerIndexMessage5, Opcodes.PlayerIndexMessage6})]
    public class PlayerIndexMessage : GameMessage
    {
        public int Field0;

        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadInt(4) + (-1);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(4, Field0 - (-1));
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PlayerIndexMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: 0x" + Field0.ToString("X8") + " (" + Field0 + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}