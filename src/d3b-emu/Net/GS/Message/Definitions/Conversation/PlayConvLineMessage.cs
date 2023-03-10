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

namespace D3BEmu.Net.GS.Message.Definitions.Conversation
{
    /// <summary>
    /// Plays a single line from a conversation.
    /// </summary>
    [Message(Opcodes.PlayConvLineMessage)]
    public class PlayConvLineMessage : GameMessage
    {
        public uint ActorID;             // The SNO of this actor is used, to get a localized "Name" of the conversation participant for chat ouput
        // MaxLength = 9
        public uint[] Field1;            // looks like a list of conversation participants - farmy
        public PlayLineParams Params;
        public int Duration;             // playback duration in ms

        public PlayConvLineMessage() : base(Opcodes.PlayConvLineMessage) {}

        public override void Parse(GameBitBuffer buffer)
        {
            ActorID = buffer.ReadUInt(32);
            Field1 = new uint[9];
            for (int i = 0; i < Field1.Length; i++) Field1[i] = buffer.ReadUInt(32);
            Params = new PlayLineParams();
            Params.Parse(buffer);
            Duration = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ActorID);
            for (int i = 0; i < Field1.Length; i++) buffer.WriteUInt(32, Field1[i]);
            Params.Encode(buffer);
            buffer.WriteInt(32, Duration);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PlayConvLineMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorID.ToString("X8") + " (" + ActorID + ")");
            b.Append(' ', pad); b.AppendLine("Field1:");
            b.Append(' ', pad); b.AppendLine("{");
            for (int i = 0; i < Field1.Length; ) { b.Append(' ', pad + 1); for (int j = 0; j < 8 && i < Field1.Length; j++, i++) { b.Append("0x" + Field1[i].ToString("X8") + ", "); } b.AppendLine(); }
            b.Append(' ', pad); b.AppendLine("}"); b.AppendLine();
            Params.AsText(b, pad);
            b.Append(' ', pad); b.AppendLine("Duration: 0x" + Duration.ToString("X8") + " (" + Duration + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}