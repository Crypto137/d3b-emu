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

namespace D3BEmu.Net.GS.Message.Definitions.Game
{
    [Message(Opcodes.VersionsMessage)]
    public class VersionsMessage : GameMessage
    {
        public int SNOPackHash;
        public int ProtocolHash;
        public string Version;

        public VersionsMessage(int snoPacketHash):base(Opcodes.VersionsMessage)
        {
            this.SNOPackHash = snoPacketHash;
            this.ProtocolHash = ImplementedProtocolHash;
            this.Version = "0.3.0.7333"; // beta patch-3
            //this.Version = "0.3.0.7484"; // beta patch-4
        }

        public VersionsMessage():base(Opcodes.VersionsMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            SNOPackHash = buffer.ReadInt(32);
            ProtocolHash = buffer.ReadInt(32);
            Version = buffer.ReadCharArray(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, SNOPackHash);
            buffer.WriteInt(32, ProtocolHash);
            buffer.WriteCharArray(32, Version);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("VersionsMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("SNOPackHash: 0x" + SNOPackHash.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("ProtocolHash: 0x" + ProtocolHash.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("Version: \"" + Version + "\"");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}