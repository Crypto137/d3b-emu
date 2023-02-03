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
    [Message(Opcodes.PlayerLevel)]
    public class PlayerLevel : GameMessage
    {
        public PlayerLevel() : base(Opcodes.PlayerLevel) { }

        public int PlayerIndex;
        public int Level;

        public override void Parse(GameBitBuffer buffer)
        {
            PlayerIndex = buffer.ReadInt(3);
            Level = buffer.ReadInt(7);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(3, PlayerIndex);
            buffer.WriteInt(7, Level);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PlayerLevel:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("PlayerIndex: 0x" + PlayerIndex.ToString("X8") + " (" + PlayerIndex + ")");
            b.Append(' ', pad); b.AppendLine("Level: 0x" + Level.ToString("X8") + " (" + Level + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}