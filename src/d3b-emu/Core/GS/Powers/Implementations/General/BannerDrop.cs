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
using D3BEmu.Core.GS.Skills;
using D3BEmu.Core.GS.Ticker;
using D3BEmu.Core.GS.Actors;
using D3BEmu.Net.GS.Message;
using D3BEmu.Common.Helpers.Math;
using D3BEmu.Core.GS.Common.Types.Math;
using D3BEmu.Core.GS.Players;

namespace D3BEmu.Core.GS.Powers.Implementations
{
    [ImplementsPowerSNO(185040)]
    public class BannerDrop : PowerScript
    {
        public override IEnumerable<TickTimer> Run()
        {
            // We use this for the good old monster spawn hack
            // number of monsters to spawn
            int spawn_count = 10;

            // list of actorSNO values to pick from when spawning
            int[] actorSNO_values = { 4282, 3893, 6652, 5428, 5346, 6024, 5393, 5467 };
            int actorSNO = actorSNO_values[RandomHelper.Next(actorSNO_values.Length - 1)];
            Logger.Debug("BannerDrop spawning sno {0}", actorSNO);

            for (int n = 0; n < spawn_count; ++n)
            {
                Vector3D position;

                if (TargetPosition.X == 0f)
                {
                    position = new Vector3D(User.Position);
                    if ((n % 2) == 0)
                    {
                        position.X += (float)(RandomHelper.NextDouble() * 20);
                        position.Y += (float)(RandomHelper.NextDouble() * 20);
                    }
                    else
                    {
                        position.X -= (float)(RandomHelper.NextDouble() * 20);
                        position.Y -= (float)(RandomHelper.NextDouble() * 20);
                    }
                }
                else
                {
                    position = new Vector3D(TargetPosition);
                    position.X += (float)(RandomHelper.NextDouble() - 0.5) * 20;
                    position.Y += (float)(RandomHelper.NextDouble() - 0.5) * 20;
                    position.Z = User.Position.Z;
                }

                Monster mon = new Monster(User.World, actorSNO, null);
                mon.Position = position;
                mon.Scale = 1.35f;
                //scripted //mon.Attributes[GameAttribute.Hitpoints_Max_Total] = 50f;
                mon.Attributes[GameAttribute.Hitpoints_Max] = 50f;
                //scripted //mon.Attributes[GameAttribute.Hitpoints_Total_From_Level] = 0f;
                mon.Attributes[GameAttribute.Hitpoints_Cur] = 50f;
                User.World.Enter(mon);
            }

            // Give barbarians some fury to make skill testing easier
            if (User is Player && ((Player)User).Toon.Class == EmuNet.Toons.ToonClass.Barbarian)
            {
                GeneratePrimaryResource(100.0f);
            }

            yield break;
        }
    }
}