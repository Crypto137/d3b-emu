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

using CrystalMpq;
using Gibbed.IO;
using D3BEmu.Common.MPQ.FileFormats.Types;
using D3BEmu.Core.GS.Common.Types.SNO;

// Appears to work fine, created from snodata.xml - DarkLotus
namespace D3BEmu.Common.MPQ.FileFormats
{
    [FileFormat(SNOGroup.Recipe)]
    public class Recipe : FileFormat
    {
        public Header Header { get; private set; }
        public ItemSpecifierData ItemSpecifierData { get; private set; }

        public Recipe(MpqFile file)
        {
            var stream = file.Open();
            this.Header = new Header(stream);
            ItemSpecifierData = new ItemSpecifierData(stream);
            stream.Close();
        }
    }
}