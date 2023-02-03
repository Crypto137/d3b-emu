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

using System.Collections.Generic;
using System.Linq;
using D3BEmu.Common;
using D3BEmu.Core.GS.Ticker;
using D3BEmu.Core.GS.Common.Types.Math;
using D3BEmu.Net.GS.Message.Definitions.World;
using D3BEmu.Core.GS.Actors;

namespace D3BEmu.Core.GS.Powers
{
    public abstract class PowerScript : PowerContext
    {
        public Vector3D TargetPosition;
        public TargetMessage TargetMessage;

        // Called to start executing a power
        // Yields timers that signify when to continue execution.
        public abstract IEnumerable<TickTimer> Run();

        // token instance that can be yielded by Run() to indicate the power manager should stop
        // running a power implementation.
        public static readonly TickTimer StopExecution = null;


        public TargetList GetBestMeleeEnemy()
        {
            float meleeRange = 10f;  // TODO: possibly use equipped weapon range for this?

            // get all targets that could be hit by melee attack, then select the script's target if
            // it has one, otherwise use the closest target in range.
            TargetList targets = GetEnemiesInBeamDirection(User.Position, TargetPosition, meleeRange);

            Actor bestEnemy;
            if (targets.Actors.Contains(Target))
                bestEnemy = Target;
            else
                bestEnemy = targets.GetClosestTo(User.Position);

            targets.Actors.RemoveAll(actor => actor != bestEnemy);
            return targets;
        }
    }
}
