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

using D3BEmu.Core.GS.Map;
using D3BEmu.Core.GS.Common.Types.TagMap;
using D3BEmu.Net.GS.Message;
using D3BEmu.Net.GS.Message.Definitions.Misc;

namespace D3BEmu.Core.GS.Actors.Implementations
{
    /// <summary>
    /// Class that implements shrines, run power on click and send activation message
    /// </summary>
    class Shrine : Gizmo
    {
        public Shrine(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            Attributes[GameAttribute.MinimapActive] = true;
        }


        public override void OnTargeted(Players.Player player, Net.GS.Message.Definitions.World.TargetMessage message)
        {
            Logger.Warn("Shrine has no function, Powers not implemented");
            World.BroadcastIfRevealed(new ShrineActivatedMessage { ActorID = this.DynamicID }, this);

            this.Attributes[GameAttribute.Gizmo_Has_Been_Operated] = true;
            this.Attributes[GameAttribute.Gizmo_Operator_ACDID] = unchecked((int)player.DynamicID);
            this.Attributes[GameAttribute.Gizmo_State] = 1;
            Attributes.BroadcastChangedIfRevealed();
        }
    }
}
