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
using D3BEmu.Core.GS.Map;
using D3BEmu.Net.GS.Message;
using D3BEmu.Core.GS.Common.Types.TagMap;

namespace D3BEmu.Core.GS.Actors
{
    public class NPC : Living
    {
        public override ActorType ActorType { get { return ActorType.Monster; } }

        public NPC(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Field2 = 0x9;
            this.Field7 = 2;
            this.Attributes[GameAttribute.Is_NPC] = true;
        }
    }
}
