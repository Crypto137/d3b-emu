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
    /// <summary>
    /// Sent from the client when a buff is right clicked to be cancelled
    /// </summary>
    [Message(Opcodes.RequestBuffCancelMessage, Consumers.Player)]
    public class RequestBuffCancelMessage : GameMessage
    {
        public int /* sno */ PowerSNOId; // SNO of the power that activated the buff to be canceled
        public int Field1; // Might be ActorID, might be number of stacks to clear off?

        public override void Parse(GameBitBuffer buffer)
        {
            PowerSNOId = buffer.ReadInt(32);
            Field1 = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, PowerSNOId);
            buffer.WriteInt(32, Field1);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("RequestBuffCancelMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("PowerSNOId: 0x" + PowerSNOId.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("Field1: 0x" + Field1.ToString("X8") + " (" + Field1 + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}