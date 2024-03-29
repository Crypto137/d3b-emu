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

namespace D3BEmu.Net.GS.Message.Definitions.Conversation
{
    /// <summary>
    /// Sent by the server to the client after EndConversationMessage. FinishConversationMessage
    /// is, what will get rid of the three dots "..." above the actor that suggest there is more
    /// coming. (Not sent if only the player char has a role in conversation)
    /// </summary>
    [Message(Opcodes.FinishConversationMessage)]
    public class FinishConversationMessage : GameMessage
    {
        public int SNOConversation;
        public FinishConversationMessage()
            : base(Opcodes.FinishConversationMessage)
        { }

        public override void Parse(GameBitBuffer buffer)
        {
            SNOConversation = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, SNOConversation);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("FinishConversationMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("SNOConversation: 0x" + SNOConversation.ToString("X8"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}