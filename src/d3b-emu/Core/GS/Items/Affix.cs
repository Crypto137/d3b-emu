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
using D3BEmu.Common;
using D3BEmu.Common.Logging;

namespace D3BEmu.Core.GS.Items
{
    public class Affix
    {
        public static readonly Logger Logger = LogManager.CreateLogger();
        public int AffixGbid { get; set; }

        public Affix(int gbid)
        {
            AffixGbid = gbid;
        }

        public override String ToString()
        {
            return String.Format("{0}", AffixGbid);
        }

        public static Affix Parse(String affixString)
        {
            try
            {
                int gbid = int.Parse(affixString);
                var affix = new Affix(gbid);
                return affix;
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("Affix can not be parsed: {0}", affixString), e);
            }
        }

    }
}
