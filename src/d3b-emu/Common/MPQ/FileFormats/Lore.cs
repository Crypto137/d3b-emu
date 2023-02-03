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

namespace D3BEmu.Common.MPQ.FileFormats
{
    [FileFormat(SNOGroup.Lore)]
    public class Lore : FileFormat
    {
        public Header Header { get; private set; }
        public int I0 { get; private set; }
        public LoreCategory Category { get; private set; }
        public int I1 { get; private set; }
        public int I2 { get; private set; }
        public int SNOConversation { get; private set; }
        public int I3 { get; private set; }

        public Lore(MpqFile file)
        {
            var stream = file.Open();
            this.Header = new Header(stream);
            this.I0 = stream.ReadValueS32();
            this.Category = (LoreCategory)stream.ReadValueS32();
            this.I1 = stream.ReadValueS32();
            this.I2 = stream.ReadValueS32();
            this.SNOConversation = stream.ReadValueS32();
            this.I3 = stream.ReadValueS32();
            stream.Close();
        }       
    }
    public enum LoreCategory
    {
        Quest = 0,
        World,
        People,
        Bestiary,
    };
}