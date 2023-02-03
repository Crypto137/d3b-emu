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

using System;
using D3BEmu.Core.GS.Skills;

namespace D3BEmu.Core.EmuNet.Toons
{
    public class ToonSkillSet
    {
        public ToonSkillSet(int[] activeSkills, int[] passiveSkills, int[] hotbarButtons)   // Constructor for existing toon skill sets
        {
            ActiveSkills = activeSkills;
            PassiveSkills = passiveSkills;
            HotbarButtons = hotbarButtons;
        }

        public ToonSkillSet(ToonClass @class)   // Constructor for new toon skill sets
        {
            // Pick starting skills based on the class
            switch (@class)
            {
                case ToonClass.Barbarian:
                    ActiveSkills = new int[6] {
                        Skills.Barbarian.FuryGenerators.Bash,
                        Skills.Barbarian.FurySpenders.ThreateningShout,
                        Skills.None,
                        Skills.None,
                        Skills.None,
                        Skills.None
                    };
                    break;

                case ToonClass.DemonHunter:
                    ActiveSkills = new int[6] {
                        Skills.DemonHunter.Offense.HungeringArrow,
                        Skills.DemonHunter.Discipline.Caltrops,
                        Skills.None,
                        Skills.None,
                        Skills.None,
                        Skills.None
                    };
                    break;

                case ToonClass.Monk:
                    ActiveSkills = new int[6] {
                        Skills.Monk.SpiritGenerators.FistsOfThunder,
                        Skills.Monk.SpiritSpenders.BlindingFlash,
                        Skills.None,
                        Skills.None,
                        Skills.None,
                        Skills.None
                    };
                    break;

                case ToonClass.WitchDoctor:
                    ActiveSkills = new int[6] {
                        Skills.WitchDoctor.PhysicalRealm.PoisonDart,
                        Skills.WitchDoctor.Support.SummonZombieDogs,
                        Skills.None,
                        Skills.None,
                        Skills.None,
                        Skills.None
                    };
                    break;

                case ToonClass.Wizard:
                    ActiveSkills = new int[6] {
                        Skills.Wizard.RapidCast.MagicMissile,
                        Skills.Wizard.Utility.FrostNova,
                        Skills.None,
                        Skills.None,
                        Skills.None,
                        Skills.None
                    };
                    break;

                default:
                    ActiveSkills = new int[6] {
                        Skills.None,
                        Skills.None,
                        Skills.None,
                        Skills.None,
                        Skills.None,
                        Skills.None
                    };
                    break;
            }

            HotbarButtons = new int[9] {
                ActiveSkills[0],        // Left click
                Skills.BasicAttack,     // Right click
                Skills.BasicAttack,     // Left click switch (key unbound)
                Skills.BasicAttack,     // Right click switch (x)
                ActiveSkills[1],        // Bar 1
                Skills.None,            // Bar 2
                Skills.None,            // Bar 3
                Skills.None,            // Bar 4
                Skills.None             // Bar 5 (Potion)
            };

            PassiveSkills = new int[3] { -1, -1, -1 };
        }

        public int[] ActiveSkills { get; set; }
        public int[] PassiveSkills { get; set; }
        public int[] HotbarButtons { get; set; }

        public void Update(SkillSet skillSet)
        {
            for (int i = 0; i < ActiveSkills.Length; i++)       // Active skills
            {
                ActiveSkills[i] = skillSet.ActiveSkills[i];
            }

            for (int i = 0; i < PassiveSkills.Length; i++)      // Passive skills
            {
                PassiveSkills[i] = skillSet.PassiveSkills[i];
            }

            for (int i = 0; i < HotbarButtons.Length; i++)      // Hotbar buttons
            {
                HotbarButtons[i] = skillSet.HotBarSkills[i].SNOSkill;
            }
        }
    }
}