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

using System.Linq;
using D3BEmu.Core.EmuNet.Toons;
using D3BEmu.Net.GS.Message.Fields;

namespace D3BEmu.Core.GS.Skills
{
    public class SkillSet
    {
        public int[] ActiveSkills { get; set; }
        public int[] PassiveSkills { get; set; }
        public HotbarButtonData[] HotBarSkills { get; set; }

        public SkillSet(ToonSkillSet toonSkillSet)
        {
            ActiveSkills = new int[6] {
                toonSkillSet.ActiveSkills[0],
                toonSkillSet.ActiveSkills[1],
                toonSkillSet.ActiveSkills[2],
                toonSkillSet.ActiveSkills[3],
                toonSkillSet.ActiveSkills[4],
                toonSkillSet.ActiveSkills[5]
            };

            PassiveSkills = new int[3] {    // setting passive skills here crashes the client, need to figure out the reason. /raist.
                toonSkillSet.PassiveSkills[0],
                toonSkillSet.PassiveSkills[1],
                toonSkillSet.PassiveSkills[2]
            };

            HotBarSkills = new HotbarButtonData[9] {
                new HotbarButtonData { SNOSkill = toonSkillSet.HotbarButtons[0], ItemGBId = -1 }, // left-click
                new HotbarButtonData { SNOSkill = toonSkillSet.HotbarButtons[1], ItemGBId = -1 }, // right-click
                new HotbarButtonData { SNOSkill = toonSkillSet.HotbarButtons[2], ItemGBId = -1 }, // hidden-bar - left-click switch - which key??
                new HotbarButtonData { SNOSkill = toonSkillSet.HotbarButtons[3], ItemGBId = -1 }, // hidden-bar - right-click switch (press x ingame)
                new HotbarButtonData { SNOSkill = toonSkillSet.HotbarButtons[4], ItemGBId = -1 }, // bar-1
                new HotbarButtonData { SNOSkill = toonSkillSet.HotbarButtons[5], ItemGBId = -1 }, // bar-2
                new HotbarButtonData { SNOSkill = toonSkillSet.HotbarButtons[6], ItemGBId = -1 }, // bar-3
                new HotbarButtonData { SNOSkill = toonSkillSet.HotbarButtons[7], ItemGBId = -1 }, // bar-4 
                new HotbarButtonData { SNOSkill = Skills.None, ItemGBId = 0x622256D4 } // Bar 5 is always potion for now
            };
        }
    }
}
