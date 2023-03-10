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
using D3BEmu.Net.GS.Message.Fields;

namespace D3BEmu.Net.GS.Message.Definitions.NPC
{
    [Message(Opcodes.NPCInteractOptionsMessage)]
    public class NPCInteractOptionsMessage : GameMessage
    {
        public uint ActorID;
        // MaxLength = 20
        public NPCInteraction[] tNPCInteraction;
        public NPCInteractOptionsType Type;

        public NPCInteractOptionsMessage()
            : base(Opcodes.NPCInteractOptionsMessage)
        {
        }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorID = buffer.ReadUInt(32);
            tNPCInteraction = new NPCInteraction[buffer.ReadInt(5)];
            for (int i = 0; i < tNPCInteraction.Length; i++) { tNPCInteraction[i] = new NPCInteraction(); tNPCInteraction[i].Parse(buffer); }
            Type = (NPCInteractOptionsType) buffer.ReadInt(2);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ActorID);
            buffer.WriteInt(5, tNPCInteraction.Length);
            for (int i = 0; i < tNPCInteraction.Length; i++) { tNPCInteraction[i].Encode(buffer); }
            buffer.WriteInt(2, (int)Type);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("NPCInteractOptionsMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorID.ToString("X8") + " (" + ActorID + ")");
            b.Append(' ', pad); b.AppendLine("tNPCInteraction:");
            b.Append(' ', pad); b.AppendLine("{");
            for (int i = 0; i < tNPCInteraction.Length; i++) { tNPCInteraction[i].AsText(b, pad + 1); b.AppendLine(); }
            b.Append(' ', pad); b.AppendLine("}"); b.AppendLine();
            b.Append(' ', pad); b.AppendLine("Type: 0x" + ((int)Type).ToString("X8") + " (" + Type + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }

    public enum NPCInteractOptionsType
    {
        Normal = 0,
        Conversation = 1,
        Unknown2 = 2, // Works like normal? /fasbat
    }
}