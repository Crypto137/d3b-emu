﻿/*
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

using System.Collections.Generic;
using D3BEmu.Common.Helpers.Math;
using D3BEmu.Core.GS.Actors;
using D3BEmu.Core.GS.Actors.Implementations.Monsters;
using D3BEmu.Core.GS.Actors.Movement;
using D3BEmu.Core.GS.Common.Types.Math;
using D3BEmu.Core.GS.Players;
using D3BEmu.Core.GS.Actors.Actions;
using D3BEmu.Net.GS.Message;
using D3BEmu.Common.MPQ;
using D3BEmu.Core.GS.Common.Types.SNO;
using D3BEmu.Core.GS.Ticker;
using System.Linq;

namespace D3BEmu.Core.GS.AI.Brains
{
    public class MonsterBrain : Brain
    {
        // list of power SNOs that are defined for the monster
        public List<int> PresetPowers { get; private set; }

        private TickTimer _powerDelay;

        private bool _warnedNoPowers;
        private int _mpqPowerCount;

        public MonsterBrain(Actor body)
            : base(body)
        {
            this.PresetPowers = new List<int>();

            // build list of powers defined in monster mpq data
            if (body.ActorData.MonsterSNO > 0)
            {
                var monsterData = (D3BEmu.Common.MPQ.FileFormats.Monster)MPQStorage.Data.Assets[SNOGroup.Monster][body.ActorData.MonsterSNO].Data;
                _mpqPowerCount = monsterData.SkillDeclarations.Count(e => e.SNOPower != -1);
                foreach (var monsterSkill in monsterData.SkillDeclarations)
                {
                    if (Powers.PowerLoader.HasImplementationForPowerSNO(monsterSkill.SNOPower))
                    {
                        this.PresetPowers.Add(monsterSkill.SNOPower);
                    }
                }
            }
        }

        public override void Think(int tickCounter)
        {
            // this needed? /mdz
            if (this.Body is NPC) return;

            // check if in disabled state, if so cancel any action then do nothing
            if (this.Body.Attributes[GameAttribute.Frozen] ||
                this.Body.Attributes[GameAttribute.Stunned] ||
                this.Body.Attributes[GameAttribute.Blind] ||
                this.Body.World.BuffManager.GetFirstBuff<Powers.Implementations.KnockbackBuff>(this.Body) != null ||
                this.Body.World.BuffManager.GetFirstBuff<Powers.Implementations.SummonedBuff>(this.Body) != null)
            {
                if (this.CurrentAction != null)
                {
                    this.CurrentAction.Cancel(tickCounter);
                    this.CurrentAction = null;
                }
                _powerDelay = null;

                return;
            }


            // select and start executing a power if no active action
            if (this.CurrentAction == null)
            {
                // do a little delay so groups of monsters don't all execute at once
                if (_powerDelay == null)
                    _powerDelay = new SecondsTickTimer(this.Body.World.Game, (float)RandomHelper.NextDouble());


                if (_powerDelay.TimedOut)
                {
                    int powerToUse = PickPowerToUse();
                    if (powerToUse > 0)
                    {
                        this.CurrentAction = new PowerAction(this.Body, powerToUse);
                    }
                }
            }
        }

        protected virtual int PickPowerToUse()
        {
            if (!_warnedNoPowers && this.PresetPowers.Count == 0)
            {
                Logger.Info("Monster \"{0}\" has no usable powers. {1} are defined in mpq data.",
                    this.Body.ActorSNO.Name, _mpqPowerCount);
                _warnedNoPowers = true;
            }

            // randomly used an implemented power
            if (this.PresetPowers.Count > 0)
            {
                int power = this.PresetPowers[RandomHelper.Next(this.PresetPowers.Count)];
                if (Powers.PowerLoader.HasImplementationForPowerSNO(power))
                    return power;
            }

            // no usable power
            return -1;
        }

        public void AddPresetPower(int powerSNO)
        {
            if (this.PresetPowers.Contains(powerSNO))
            {
                Logger.Error("AddPresetPower(): power sno {0} already defined for monster \"{1}\"",
                    powerSNO, this.Body.ActorSNO.Name);
                return;
            }

            this.PresetPowers.Add(powerSNO);
        }

        public void RemovePresetPower(int powerSNO)
        {
            if (this.PresetPowers.Contains(powerSNO))
            {
                this.PresetPowers.Remove(powerSNO);
            }
        }
    }
}
