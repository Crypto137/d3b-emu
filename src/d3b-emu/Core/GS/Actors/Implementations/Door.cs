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
using D3BEmu.Core.GS.Map;
using D3BEmu.Core.GS.Common.Types.TagMap;
using D3BEmu.Net.GS.Message.Definitions.Animation;
using D3BEmu.Core.GS.Common.Types.SNO;
using D3BEmu.Net.GS.Message;
using TreasureClass = D3BEmu.Common.MPQ.FileFormats.TreasureClass;

namespace D3BEmu.Core.GS.Actors.Implementations
{
    /// <summary>
    /// Class that implements behaviour for clickable door types.
    /// Play open animation on click, then set idle animation
    /// </summary>
    class Door : Gizmo
    {
        public Door(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
        }


        public override void OnTargeted(Players.Player player, Net.GS.Message.Definitions.World.TargetMessage message)
        {
            World.BroadcastIfRevealed(new PlayAnimationMessage
            {
                ActorID = this.DynamicID,
                Field1 = 5,
                Field2 = 0,
                tAnim = new Net.GS.Message.Fields.PlayAnimationMessageSpec[]
                {
                    new Net.GS.Message.Fields.PlayAnimationMessageSpec()
                    {
                        Duration = 50,
                        AnimationSNO = AnimationSet.TagMapAnimDefault[AnimationSetKeys.Opening],
                        PermutationIndex = 0,
                        Speed = 1
                    }
                }

            }, this);

            World.BroadcastIfRevealed(new SetIdleAnimationMessage
            {
                ActorID = this.DynamicID,
                AnimationSNO = AnimationSetKeys.Open.ID
            }, this);

            this.Attributes[GameAttribute.Gizmo_Has_Been_Operated] = true;
            this.Attributes[GameAttribute.Gizmo_Operator_ACDID] = unchecked((int)player.DynamicID);
            this.Attributes[GameAttribute.Gizmo_State] = 1;
            Attributes.BroadcastChangedIfRevealed();

            base.OnTargeted(player, message);
        }
    }
}