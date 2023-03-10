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

namespace D3BEmu.Net.GS.Message.Definitions.Inventory
{
    [Message(Opcodes.InventorySplitStackMessage, Consumers.Inventory)]
    public class InventorySplitStackMessage : GameMessage
    {
        public int FromID;
        public long Amount;
        public InvLoc InvLoc;

        public override void Parse(GameBitBuffer buffer)
        {
            FromID = buffer.ReadInt(32);
            Amount = buffer.ReadInt64(64);
            InvLoc = new InvLoc();
            InvLoc.Parse(buffer);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, FromID);
            buffer.WriteInt64(64, Amount);
            InvLoc.Encode(buffer);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("InventorySplitStackMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("FromID: 0x" + FromID.ToString("X8") + " (" + FromID + ")");
            b.Append(' ', pad); b.AppendLine("Amount: 0x" + Amount.ToString("X16"));
            InvLoc.AsText(b, pad);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}