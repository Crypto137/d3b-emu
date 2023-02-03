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

namespace D3BEmu.Net.GS.Message.Definitions.ACD
{
    [Message(Opcodes.ACDGroupMessage)]
    public class ACDGroupMessage : GameMessage
    {
        public uint ActorID; // Actor's DynamicID
        public int Group1Hash;
        public int Group2Hash;

        public ACDGroupMessage() : base(Opcodes.ACDGroupMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorID = buffer.ReadUInt(32);
            Group1Hash = buffer.ReadInt(32);
            Group2Hash = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ActorID);
            buffer.WriteInt(32, Group1Hash);
            buffer.WriteInt(32, Group2Hash);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ACDGroupMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorID.ToString("X8") + " (" + ActorID + ")");
            b.Append(' ', pad); b.AppendLine("Group1Hash: 0x" + Group1Hash.ToString("X8") + " (" + Group1Hash + ")");
            b.Append(' ', pad); b.AppendLine("Group2Hash: 0x" + Group2Hash.ToString("X8") + " (" + Group2Hash + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
