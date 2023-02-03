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
using CrystalMpq;
using Gibbed.IO;
using D3BEmu.Common.MPQ.FileFormats.Types;
using D3BEmu.Core.GS.Common.Types.SNO;

namespace D3BEmu.Common.MPQ.FileFormats
{
    [FileFormat(SNOGroup.Encounter)]
    public class Encounter : FileFormat
    {
        public Header Header { get; private set; }
        public int SNOSpawn { get; private set; }
        public List<EncounterSpawnOptions> Spawnoptions = new List<EncounterSpawnOptions>();

        public Encounter(MpqFile file)
        {
            var stream = file.Open();
            this.Header = new Header(stream);
            this.SNOSpawn = stream.ReadValueS32();
            stream.Position += (2 * 4);// pad 2 int
            this.Spawnoptions = stream.ReadSerializedData<EncounterSpawnOptions>();
            stream.Close();
        }
    }

    public class EncounterSpawnOptions : ISerializableData
    {
        public int SNOSpawn { get; private set; }
        public int Probability { get; private set; }
        public int I1 { get; private set; }

        public void Read(MpqFileStream stream)
        {
            this.SNOSpawn = stream.ReadValueS32();
            this.Probability = stream.ReadValueS32();
            this.I1 = stream.ReadValueS32();
        }
    }
}
