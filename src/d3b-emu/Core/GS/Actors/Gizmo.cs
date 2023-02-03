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
using D3BEmu.Common.Logging;

namespace D3BEmu.Core.GS.Actors
{
    public class Gizmo : Actor
    {
        public override ActorType ActorType { get { return ActorType.Gizmo; } }
        protected Logger Logger = new Logger("Gizmo");

        public Gizmo(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Field2 = 16;
            this.Field7 = 0x00000001;

            this.Attributes[D3BEmu.Net.GS.Message.GameAttribute.Hitpoints_Cur] = 1;
        }
    }
}
