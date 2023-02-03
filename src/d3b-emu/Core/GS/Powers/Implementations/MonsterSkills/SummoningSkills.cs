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
using D3BEmu.Common.Helpers.Math;
using D3BEmu.Core.GS.Actors.Movement;
using D3BEmu.Core.GS.Common.Types.Math;
using D3BEmu.Core.GS.Ticker;
using D3BEmu.Net.GS.Message;
using D3BEmu.Core.GS.Actors;
using D3BEmu.Core.GS.Common.Types.TagMap;

namespace D3BEmu.Core.GS.Powers.Implementations
{
    public abstract class SummoningSkill : ActionTimedSkill
    {
        public Vector3D SpawnPosition { get; set; }

        protected void RandomPostion() // spawn actor at random postion
        {
            this.SpawnPosition = RandomDirection(User.Position, 0, 1);
        }

        protected void UserPostion() // spawn actor at user postion
        {
            this.SpawnPosition = User.Position;
        }

        protected void InFrontPostion() // spawn actor in front of user
        {
            float userFacing = (float)Math.Acos(this.User.RotationW) * 2f;
            this.SpawnPosition = new Vector3D(User.Position.X + 8 * (float)Math.Cos(userFacing),
                                             User.Position.Y + 8 * (float)Math.Sin(userFacing),
                                             User.Position.Z);
        }

        public void SummonMonster(int actorSNO)
        {
            var monster = ActorFactory.Create(User.World, actorSNO, new TagMap());
            monster.Scale = 1.35f;  // TODO: look this up properly
            monster.EnterWorld(this.SpawnPosition);
            this.World.BuffManager.AddBuff(User, monster, new Implementations.SummonedBuff());
        }
    }

    [ImplementsPowerSNO(94734)] // Summon_Zombie_Vomit.pow
    public class WretchedMotherVomit : SummoningSkill
    {
        public override IEnumerable<TickTimer> Main()
        {
            InFrontPostion();
            SummonMonster((this.User as Monster).SNOSummons[0]);
            yield break;
        }
    }

    [ImplementsPowerSNO(30543)] // Summon Skeleton
    public class SummonSkeleton : SummoningSkill
    {
        public override IEnumerable<TickTimer> Main()
        {
            RandomPostion();
            SummonMonster((this.User as Monster).SNOSummons[0]);
            yield break;
        }
    }

    [ImplementsPowerSNO(30800)] // Summon Spores
    public class SummonSpores : SummoningSkill
    {
        public override IEnumerable<TickTimer> Main()
        {
            RandomPostion();
            SummonMonster(5482);  // HACK: we don't have this in mpq
            yield break;
        }
    }
    [ImplementsPowerSNO(117580)] // Summon FleshPitFlyers
    public class SummonFleshPitFlyers : SummoningSkill
    {
        public override IEnumerable<TickTimer> Main()
        {
            UserPostion();
            SummonMonster((this.User as Monster).SNOSummons[0]);
            yield break;
        }
    }
    //
}
