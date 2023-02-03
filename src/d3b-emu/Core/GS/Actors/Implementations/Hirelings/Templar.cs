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
using D3BEmu.Common.Helpers.Hash;
using D3BEmu.Common.MPQ.FileFormats.Types;
using D3BEmu.Core.GS.Common.Types.Math;
using D3BEmu.Core.GS.Map;
using D3BEmu.Net.GS.Message;
using D3BEmu.Common.Helpers;
using D3BEmu.Core.GS.Common.Types.TagMap;

namespace D3BEmu.Core.GS.Actors.Implementations.Hirelings
{
    [HandledSNO(4538 /* Templar.acr */)]
    public class Templar : Hireling
    {
        public Templar(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            mainSNO = 4538;
            hirelingSNO = 0x0000CDD5;  
            proxySNO = 0x0002F1AC;
            skillKit = 0x8AFB;
            hirelingGBID = StringHashHelper.HashItemName("Templar");
            this.Attributes[GameAttribute.Hireling_Class] = 1;
        }

        public override Hireling CreateHireling(World world, int snoId, TagMap tags)
        {
            return new Templar(world, snoId, tags);
        }
    }   
}