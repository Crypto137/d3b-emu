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

namespace D3BEmu.Net.GS.Message.Definitions.Skill
{
    [Message(Opcodes.AssignSkillMessage1, Consumers.Player)]
    public class AssignActiveSkillMessage : GameMessage
    {
        public int /* sno */ SNOSkill;
        public int SkillIndex;

        public override void Parse(GameBitBuffer buffer)
        {
            SNOSkill = buffer.ReadInt(32);
            SkillIndex = buffer.ReadInt(5);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, SNOSkill);
            buffer.WriteInt(5, SkillIndex);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("AssignSkillMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("snoPower: 0x" + SNOSkill.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("Field1: 0x" + SkillIndex.ToString("X8") + " (" + SkillIndex + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
