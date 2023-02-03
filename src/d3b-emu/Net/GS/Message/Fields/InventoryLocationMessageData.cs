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
using D3BEmu.Core.GS.Common.Types.Math;

namespace D3BEmu.Net.GS.Message.Fields
{
    public class InventoryLocationMessageData
    {
        public uint OwnerID; // Player's DynamicID
        public int EquipmentSlot;
        public Vector2D InventoryLocation; // Row, column

        public void Parse(GameBitBuffer buffer)
        {
            OwnerID = buffer.ReadUInt(32);
            EquipmentSlot = buffer.ReadInt(5) + (-1);
            InventoryLocation = new Vector2D();
            InventoryLocation.Parse(buffer);
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, OwnerID);
            buffer.WriteInt(5, EquipmentSlot - (-1));
            InventoryLocation.Encode(buffer);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("InventoryLocationMessageData:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("OwnerID: 0x" + OwnerID.ToString("X8") + " (" + OwnerID + ")");
            b.Append(' ', pad);
            b.AppendLine("EquipmentSlot: 0x" + EquipmentSlot.ToString("X8") + " (" + EquipmentSlot + ")");
            InventoryLocation.AsText(b, pad);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
