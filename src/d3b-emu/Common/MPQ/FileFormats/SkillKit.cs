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
    [FileFormat(SNOGroup.SkillKit)]
    public class SkillKit : FileFormat
    {
        public Header Header { get; private set; }
        public List<TraitEntry> TraitEntries { get; private set; }
        public List<ActiveSkillEntry> ActiveSkillEntries { get; private set; }

        public SkillKit(MpqFile file)
        {
            var stream = file.Open();
            this.Header = new Header(stream);
            stream.Position += 12;
            this.TraitEntries = stream.ReadSerializedData<TraitEntry>();
            stream.Position += 8;
            this.ActiveSkillEntries = stream.ReadSerializedData<ActiveSkillEntry>();
            stream.Close();
        }
    }

    public class TraitEntry : ISerializableData
    {
        public int SNOPower { get; private set; }
        public int Category { get; private set; }
        public int ReqLevel { get; private set; }

        public void Read(MpqFileStream stream)
        {
            this.SNOPower = stream.ReadValueS32();
            this.Category = stream.ReadValueS32();
            this.ReqLevel = stream.ReadValueS32();
        }
    }

    public class ActiveSkillEntry : ISerializableData
    {
        public int SNOPower { get; private set; }
        public ActiveSkillCategory Category { get; private set; }
        public int ReqLevel { get; private set; }
        public int I2 { get; private set; }

        public void Read(MpqFileStream stream)
        {
            this.SNOPower = stream.ReadValueS32();
            this.Category = (ActiveSkillCategory)stream.ReadValueS32();
            this.ReqLevel = stream.ReadValueS32();
            this.I2 = stream.ReadValueS32();

        }
    }

    public enum ActiveSkillCategory
    {
        FuryGenerator = 0,
        FurySpender,
        Situational,
        Signature,
        Offensive,
        Utility,
        PhysicalRealm,
        SpiritRealm,
        Support,
        HatredGenerator,
        HatredSpender,
        Discipline,
        SpiritGenerator,
        SpiritSpender,
        Mantras,
    }
}
