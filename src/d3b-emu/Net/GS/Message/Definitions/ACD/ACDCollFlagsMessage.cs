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
    [Message(Opcodes.ACDCollFlagsMessage)]
    public class ACDCollFlagsMessage : GameMessage
    {
        public uint ActorID; // The actor's DynamicID
        public int CollFlags;

        public ACDCollFlagsMessage() : base(Opcodes.ACDCollFlagsMessage) {}

        public override void Parse(GameBitBuffer buffer)
        {
            ActorID = buffer.ReadUInt(32);
            CollFlags = buffer.ReadInt(12);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ActorID);
            buffer.WriteInt(12, CollFlags);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ACDCollFlagsMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorID.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("CollFlags: 0x" + CollFlags.ToString("X8") + " (" + CollFlags + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
