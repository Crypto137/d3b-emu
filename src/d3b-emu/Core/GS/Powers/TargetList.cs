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
using D3BEmu.Core.GS.Actors;
using D3BEmu.Core.GS.Common.Types.Math;

namespace D3BEmu.Core.GS.Powers
{
    public class TargetList
    {
        // list of actors that are the primary targets
        public List<Actor> Actors { get; private set; }

        // list of extra actors that are near the targets, i.e. destructables like barrels, tombstones etc.
        public List<Actor> ExtraActors { get; private set; }

        public TargetList()
        {
            this.Actors = new List<Actor>();
            this.ExtraActors = new List<Actor>();
        }

        public void SortByDistanceFrom(Vector3D position)
        {
            this.Actors = this.Actors.OrderBy(actor => PowerMath.Distance2D(actor.Position, position)).ToList();
        }

        public Actor GetClosestTo(Vector3D position)
        {
            Actor closest = null;
            float closestDistance = float.MaxValue;
            foreach (Actor actor in this.Actors)
            {
                float distance = PowerMath.Distance2D(actor.Position, position);
                if (distance < closestDistance)
                {
                    closest = actor;
                    closestDistance = distance;
                }
            }

            return closest;
        }
    }
}
