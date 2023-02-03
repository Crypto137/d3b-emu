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

namespace D3BEmu.Net.GS.Message.Definitions.Conversation
{
    /// <summary>
    /// Client -> Server
    /// 
    /// Sent by the client to request stopping playback of the current line
    /// and continue with the next line of the conversation
    /// (Player closes the conversation dialog window)
    /// </summary>
    [Message(Opcodes.AdvanceConvMessage, Consumers.Conversations)]
    class AdvanceConvMessage : GameMessage
    {
        // Looks like the client doesn't send conversation SNO as a separate field in 7447

        /// <summary>
        /// Identifier of the PlayLineParams as used in PlayConvLineMessage to start the conversation
        /// </summary>
        public int PlayLineParamsId;

        public override void Parse(GameBitBuffer buffer)
        {
            PlayLineParamsId = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, PlayLineParamsId);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("AdvanceConvMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("PlayLineParamsId: 0x" + PlayLineParamsId.ToString("X8") + " (" + PlayLineParamsId + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}