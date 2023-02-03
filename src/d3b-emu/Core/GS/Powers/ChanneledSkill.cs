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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using D3BEmu.Core.GS.Ticker;

namespace D3BEmu.Core.GS.Powers
{
    public abstract class ChanneledSkill : PowerScript
    {
        public bool IsChannelOpen = false;
        public float EffectsPerSecond = 1.0f;

        public virtual void OnChannelOpen() { }
        public virtual void OnChannelClose() { }
        public virtual void OnChannelUpdated() { }
        public abstract IEnumerable<TickTimer> Main();

        private TickTimer _effectTimeout = null;

        public sealed override IEnumerable<TickTimer> Run()
        {
            // process channeled skill events
            if (IsChannelOpen)
            {
                OnChannelUpdated();
            }
            else  // first call to this skill's Run(), set channel as open
            {
                OnChannelOpen();
                IsChannelOpen = true;
            }

            // run main script if ready
            if (_effectTimeout == null || _effectTimeout.TimedOut)
            {
                _effectTimeout = WaitSeconds(EffectsPerSecond);
                foreach (TickTimer timeout in Main())
                    yield return timeout;
            }
        }

        public void CloseChannel()
        {
            OnChannelClose();
            IsChannelOpen = false;
        }
    }
}
