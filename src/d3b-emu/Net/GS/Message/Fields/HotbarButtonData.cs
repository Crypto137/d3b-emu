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

namespace D3BEmu.Net.GS.Message.Fields
{
    public class HotbarButtonData
    {
        public int /* sno */ SNOSkill;
        public int /* gbid */ ItemGBId;

        public void Parse(GameBitBuffer buffer)
        {
            SNOSkill = buffer.ReadInt(32);
            ItemGBId = buffer.ReadInt(32);
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, SNOSkill);
            buffer.WriteInt(32, ItemGBId);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("HotbarButtonData:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("m_snoPower: 0x" + SNOSkill.ToString("X8"));
            b.Append(' ', pad);
            b.AppendLine("m_gbidItem: 0x" + ItemGBId.ToString("X8"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}