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
using D3BEmu.Common.Helpers.Math;
using D3BEmu.Core.GS.Map;
using D3BEmu.Core.GS.Players;
using D3BEmu.Core.GS.Powers;
using D3BEmu.Core.GS.Powers.Payloads;
using D3BEmu.Net.GS.Message;
using D3BEmu.Net.GS.Message.Definitions.Animation;
using D3BEmu.Core.GS.Common.Types.TagMap;
using D3BEmu.Core.GS.Common.Types.SNO;

namespace D3BEmu.Core.GS.Actors
{
    public class Living : Actor
    {
        public override ActorType ActorType { get { return ActorType.Monster; } }

        public SNOHandle Monster { get; private set; }

        /// <summary>
        /// The brain for 
        /// </summary>
        public AI.Brain Brain { get; protected set; }

        public Living(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Monster = new SNOHandle(SNOGroup.Monster, (ActorData.MonsterSNO));

            // FIXME: This is hardcoded crap
            this.SetFacingRotation((float)(RandomHelper.NextDouble() * 2.0f * Math.PI));
            this.GBHandle.Type = -1; this.GBHandle.GBID = -1;
            this.Field7 = 0x00000001;
            this.Field10 = 0x0;

            //scripted //this.Attributes[GameAttribute.Hitpoints_Max_Total] = 4.546875f;
            this.Attributes[GameAttribute.Hitpoints_Max] = 4.546875f;
            //scripted //this.Attributes[GameAttribute.Hitpoints_Total_From_Level] = 0f;
            this.Attributes[GameAttribute.Hitpoints_Cur] = 4.546875f;

            this.Attributes[GameAttribute.Level] = 1;
        }

        public override bool Reveal(Player player)
        {
            if (!base.Reveal(player))
                return false;
            if (AnimationSet != null)
            {
                if (this.AnimationSet.GetAnimationTag(D3BEmu.Common.MPQ.FileFormats.AnimationTags.Idle) != -1)
                {
                    player.InGameClient.SendMessage(new SetIdleAnimationMessage
                    {
                        ActorID = this.DynamicID,
                        AnimationSNO = this.AnimationSet.GetAnimationTag(D3BEmu.Common.MPQ.FileFormats.AnimationTags.Idle)
                    });
                }

            }
            return true;
        }

        public void Kill(PowerContext context = null, bool lootAndExp = false)
        {
            var deathload = new DeathPayload(context, Powers.DamageType.Physical, this, lootAndExp);
            deathload.Apply();
        }
    }
}
