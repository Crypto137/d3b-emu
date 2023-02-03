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

namespace D3BEmu.Net.GS.Message.Definitions.Inventory
{
    [Message(Opcodes.InventoryStackTransferMessage, Consumers.Inventory)]
    public class InventoryStackTransferMessage : GameMessage
    {
        public uint FromID;
        public uint ToID;
        public ulong Amount;

        public override void Parse(GameBitBuffer buffer)
        {
            FromID = buffer.ReadUInt(32);
            ToID = buffer.ReadUInt(32);
            Amount = buffer.ReadUInt64(64);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, FromID);
            buffer.WriteUInt(32, ToID);
            buffer.WriteUInt64(64, Amount);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("InventoryStackTransferMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("FromID: 0x" + FromID.ToString("X8") + " (" + FromID + ")");
            b.Append(' ', pad); b.AppendLine("ToID: 0x" + ToID.ToString("X8") + " (" + ToID + ")");
            b.Append(' ', pad); b.AppendLine("Amount: 0x" + Amount.ToString("X16"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
