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
    [Message(Opcodes.InventoryRequestUseMessage, Consumers.Inventory)]
    public class InventoryRequestUseMessage : GameMessage
    {
        public uint UsedItem;
        public int Field1;
        public uint UsedOnItem;
        public WorldPlace Location;

        public override void Parse(GameBitBuffer buffer)
        {
            UsedItem = buffer.ReadUInt(32);
            Field1 = buffer.ReadInt(2) + (-1);
            UsedOnItem = buffer.ReadUInt(32);
            Location = new WorldPlace();
            Location.Parse(buffer);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, UsedItem);
            buffer.WriteInt(2, Field1 - (-1));
            buffer.WriteUInt(32, UsedOnItem);
            Location.Encode(buffer);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("InventoryRequestUseMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("UsedItem: 0x" + UsedItem.ToString("X8") + " (" + UsedItem + ")");
            b.Append(' ', pad); b.AppendLine("Field1: 0x" + Field1.ToString("X8") + " (" + Field1 + ")");
            b.Append(' ', pad); b.AppendLine("UsedOnItem: 0x" + UsedOnItem.ToString("X8") + " (" + UsedOnItem + ")");
            Location.AsText(b, pad);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}