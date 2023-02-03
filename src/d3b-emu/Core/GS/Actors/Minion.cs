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
using D3BEmu.Common.Helpers;
using D3BEmu.Common.Helpers.Math;
using D3BEmu.Common.MPQ.FileFormats.Types;
using D3BEmu.Core.GS.Map;
using D3BEmu.Core.GS.Objects;
using D3BEmu.Core.GS.Players;
using D3BEmu.Net.GS.Message;
using D3BEmu.Net.GS.Message.Definitions.World;
using D3BEmu.Net.GS.Message.Fields;
using D3BEmu.Net.GS.Message.Definitions.Animation;
using D3BEmu.Net.GS.Message.Definitions.Effect;
using D3BEmu.Net.GS.Message.Definitions.Misc;
using D3BEmu.Common.MPQ;
using D3BEmu.Core.GS.Common.Types.SNO;
using System;
using D3BEmu.Core.GS.Common.Types.TagMap;
using MonsterFF = D3BEmu.Common.MPQ.FileFormats.Monster;
using ActorFF = D3BEmu.Common.MPQ.FileFormats.Actor;
using D3BEmu.Core.GS.AI.Brains;
using D3BEmu.Core.GS.Ticker;


namespace D3BEmu.Core.GS.Actors
{
    public class Minion : Living, IUpdateable
    {
        public Actor Master; //The player who summoned the minion.

        public override ActorType ActorType { get { return ActorType.Monster; } }

        public override int Quality
        {
            get
            {
                return (int)D3BEmu.Common.MPQ.FileFormats.SpawnType.Normal; //Seems like this was never implemented on the clientside, so using 0 is fine.
            }
            set
            {
                // Not implemented
            }
        }

        public Minion(World world, int snoId, Actor master, TagMap tags)
            : base(world, snoId, tags)
        {
            // The following two seems to be shared with monsters. One wonders why there isn't a specific actortype for minions.
            this.Master = master;
            this.Field2 = 0x8; 
            this.GBHandle.Type = (int)GBHandleType.Monster; this.GBHandle.GBID = 1;
            this.Attributes[GameAttribute.Summoned_By_ACDID] = (int)master.DynamicID;
            this.Attributes[GameAttribute.TeamID] = master.Attributes[GameAttribute.TeamID];
        }

        public override void OnTargeted(Player player, TargetMessage message)
        {
        }

        public void Update(int tickCounter)
        {
            if (this.Brain == null)
                return;

            this.Brain.Update(tickCounter);
        }

        public void SetBrain(D3BEmu.Core.GS.AI.Brain brain)
        {
            this.Brain = brain;
        }

        public void AddPresetPower(int powerSNO)
        {
            //(Brain as MinionBrain).AddPresetPower(powerSNO);
        }
    }
}
