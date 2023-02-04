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
    public class PlayerSavedData
    {
        // MaxLength = 9
        public HotbarButtonData[] HotbarButtons;
        // MaxLength = 15
        public SkillKeyMapping[] SkillKeyMappings;
        public int PlaytimeTotal;
        public int WaypointFlags;
        public HirelingSavedData HirelingSavedData;
        public int Field5;
        public LearnedLore LearnedLore;
        // MaxLength = 6
        public int /* sno */[] snoActiveSkills;
        // MaxLength = 3
        public int /* sno */[] snoTraits;
        public SavePointData SavePointData;
        // MaxLength = 64
        public int /* sno */[] m_SeenTutorials;

        public void Parse(GameBitBuffer buffer)
        {
            HotbarButtons = new HotbarButtonData[9];
            for (int i = 0; i < HotbarButtons.Length; i++)
            {
                HotbarButtons[i] = new HotbarButtonData();
                HotbarButtons[i].Parse(buffer);
            }
            SkillKeyMappings = new SkillKeyMapping[15];
            for (int i = 0; i < SkillKeyMappings.Length; i++)
            {
                SkillKeyMappings[i] = new SkillKeyMapping();
                SkillKeyMappings[i].Parse(buffer);
            }
            PlaytimeTotal = buffer.ReadInt(32);
            WaypointFlags = buffer.ReadInt(32);
            HirelingSavedData = new HirelingSavedData();
            HirelingSavedData.Parse(buffer);
            Field5 = buffer.ReadInt(32);
            LearnedLore = new LearnedLore();
            LearnedLore.Parse(buffer);
            snoActiveSkills = new int /* sno */[6];
            for (int i = 0; i < snoActiveSkills.Length; i++) snoActiveSkills[i] = buffer.ReadInt(32);
            snoTraits = new int /* sno */[3];
            for (int i = 0; i < snoTraits.Length; i++) snoTraits[i] = buffer.ReadInt(32);
            SavePointData = new SavePointData();
            SavePointData.Parse(buffer);
            m_SeenTutorials = new int /* sno */[64];
            for (int i = 0; i < m_SeenTutorials.Length; i++) m_SeenTutorials[i] = buffer.ReadInt(32);
        }

        public void Encode(GameBitBuffer buffer)
        {
            for (int i = 0; i < HotbarButtons.Length; i++)
            {
                HotbarButtons[i].Encode(buffer);
            }
            for (int i = 0; i < SkillKeyMappings.Length; i++)
            {
                SkillKeyMappings[i].Encode(buffer);
            }
            buffer.WriteInt(32, PlaytimeTotal);
            buffer.WriteInt(32, WaypointFlags);
            HirelingSavedData.Encode(buffer);
            buffer.WriteInt(32, Field5);
            LearnedLore.Encode(buffer);
            for (int i = 0; i < snoActiveSkills.Length; i++) buffer.WriteInt(32, snoActiveSkills[i]);
            for (int i = 0; i < snoTraits.Length; i++) buffer.WriteInt(32, snoTraits[i]);
            SavePointData.Encode(buffer);
            for (int i = 0; i < m_SeenTutorials.Length; i++) buffer.WriteInt(32, m_SeenTutorials[i]);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PlayerSavedData:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("Field0:");
            b.Append(' ', pad);
            b.AppendLine("{");
            for (int i = 0; i < HotbarButtons.Length; i++)
            {
                HotbarButtons[i].AsText(b, pad + 1);
                b.AppendLine();
            }
            b.Append(' ', pad);
            b.AppendLine("}");
            b.AppendLine();
            b.Append(' ', pad);
            b.AppendLine("Field1:");
            b.Append(' ', pad);
            b.AppendLine("{");
            for (int i = 0; i < SkillKeyMappings.Length; i++)
            {
                SkillKeyMappings[i].AsText(b, pad + 1);
                b.AppendLine();
            }
            b.Append(' ', pad);
            b.AppendLine("}");
            b.AppendLine();
            b.Append(' ', pad);
            b.AppendLine("PlaytimeTotal: 0x" + PlaytimeTotal.ToString("X8"));
            b.Append(' ', pad);
            b.AppendLine("WaypointFlags: 0x" + WaypointFlags.ToString("X8") + " (" + WaypointFlags + ")");
            HirelingSavedData.AsText(b, pad);
            b.Append(' ', pad);
            b.AppendLine("Field5: 0x" + Field5.ToString("X8") + " (" + Field5 + ")");
            LearnedLore.AsText(b, pad);
            b.Append(' ', pad);
            b.AppendLine("snoActiveSkills:");
            b.Append(' ', pad);
            b.AppendLine("{");
            for (int i = 0; i < snoActiveSkills.Length;)
            {
                b.Append(' ', pad + 1);
                for (int j = 0; j < 8 && i < snoActiveSkills.Length; j++, i++)
                {
                    b.Append("0x" + snoActiveSkills[i].ToString("X8") + ", ");
                }
                b.AppendLine();
            }
            b.Append(' ', pad);
            b.AppendLine("}");
            b.AppendLine();
            b.Append(' ', pad);
            b.AppendLine("snoTraits:");
            b.Append(' ', pad);
            b.AppendLine("{");
            for (int i = 0; i < snoTraits.Length;)
            {
                b.Append(' ', pad + 1);
                for (int j = 0; j < 8 && i < snoTraits.Length; j++, i++)
                {
                    b.Append("0x" + snoTraits[i].ToString("X8") + ", ");
                }
                b.AppendLine();
            }
            b.Append(' ', pad);
            b.AppendLine("}");
            b.AppendLine();
            SavePointData.AsText(b, pad);
            b.Append(' ', pad);
            b.AppendLine("m_SeenTutorials:");
            b.Append(' ', pad);
            b.AppendLine("{");
            for (int i = 0; i < m_SeenTutorials.Length;)
            {
                b.Append(' ', pad + 1);
                for (int j = 0; j < 8 && i < m_SeenTutorials.Length; j++, i++)
                {
                    b.Append("0x" + m_SeenTutorials[i].ToString("X8") + ", ");
                }
                b.AppendLine();
            }
            b.Append(' ', pad);
            b.AppendLine("}");
            b.AppendLine();
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}