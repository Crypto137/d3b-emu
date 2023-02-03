﻿/*
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
    [FileFormat(SNOGroup.LevelArea)]
    public class LevelArea : FileFormat
    {
        public Header Header { get; private set; }
        public int I0 { get; private set; }
        public int I1 { get; private set; }
        public int SNOLevelAreaOverrideForGizmoLocs { get; private set; }
        public GizmoLocSet LocSet { get; private set; }
        public int I2 { get; private set; }
        public List<LevelAreaSpawnPopulation> SpawnPopulation { get; private set; }

        public LevelArea(MpqFile file)
        {
            var stream = file.Open();
            this.Header = new Header(stream);
            this.I0 = stream.ReadValueS32();
            this.I1 = stream.ReadValueS32();
            this.SNOLevelAreaOverrideForGizmoLocs = stream.ReadValueS32();
            this.LocSet = new GizmoLocSet(stream);
            this.I2 = stream.ReadValueS32();
            stream.Position += 12;
            this.SpawnPopulation = stream.ReadSerializedData<LevelAreaSpawnPopulation>();
            stream.Close();
        }
    }

    public class GizmoLocSet
    {
        public GizmoLocSpawnType[] SpawnType { get; private set; }

        public GizmoLocSet(MpqFileStream stream)
        {
            this.SpawnType = new GizmoLocSpawnType[26];
            for (int i = 0; i < 26; i++)
                this.SpawnType[i] = new GizmoLocSpawnType(stream);
        }
    }

    public class GizmoLocSpawnType
    {
        public List<GizmoLocSpawnEntry> SpawnEntry { get; private set; }
        public string Description { get; private set; }
        public string Comment { get; private set; }

        public GizmoLocSpawnType(MpqFileStream stream)
        {
            stream.Position += 8;
            this.SpawnEntry = stream.ReadSerializedData<GizmoLocSpawnEntry>();
            this.Description = stream.ReadString(80, true);
            this.Comment = stream.ReadString(256, true);
        }
    }

    public class GizmoLocSpawnEntry : ISerializableData
    {
        public int Min { get; private set; }
        public int Max { get; private set; }
        public SNOHandle SNOHandle { get; private set; }
        public int I2 { get; private set; }

        public void Read(MpqFileStream stream)
        {
            this.Min = stream.ReadValueS32();
            this.Max = stream.ReadValueS32();
            this.SNOHandle = new SNOHandle(stream);
            this.I2 = stream.ReadValueS32();
        }
    }

    public class LevelAreaSpawnPopulation : ISerializableData
    {
        public string S0 { get; private set; }
        public int I0 { get; private set; }
        public int[] I1 { get; private set; }
        public int I2 { get; private set; }
        public List<LevelAreaSpawnGroup> SpawnGroup { get; private set; }

        public void Read(MpqFileStream stream)
        {
            this.S0 = stream.ReadString(64, true);
            this.I0 = stream.ReadValueS32();
            this.I1 = new int[4];
            for (int i = 0; i < 4; i++)
                this.I1[i] = stream.ReadValueS32();
            this.I2 = stream.ReadValueS32();
            stream.Position += 8;
            this.SpawnGroup = stream.ReadSerializedData<LevelAreaSpawnGroup>();
        }
    }

    public class LevelAreaSpawnGroup : ISerializableData
    {
        public SpawnGroupType GroupType { get; private set; }
        public float F0 { get; private set; }
        public float F1 { get; private set; }
        public int I0 { get; private set; }
        public int I1 { get; private set; }
        public List<LevelAreaSpawnItem> SpawnItems { get; private set; }
        public int I2 { get; private set; }
        public int I3 { get; private set; }

        public void Read(MpqFileStream stream)
        {
            this.GroupType = (SpawnGroupType)stream.ReadValueS32();
            this.F0 = stream.ReadValueF32();
            this.F1 = stream.ReadValueF32();
            this.I0 = stream.ReadValueS32();
            this.I1 = stream.ReadValueS32();
            stream.Position += 12;
            this.SpawnItems = stream.ReadSerializedData<LevelAreaSpawnItem>();
            this.I2 = stream.ReadValueS32();
            this.I3 = stream.ReadValueS32();
        }
    }

    public enum SpawnGroupType : int
    {
        Density = 0,
        Exactly = 1,
    }

    public class LevelAreaSpawnItem : ISerializableData
    {
        public SNOHandle SNOHandle { get; private set; }
        public SpawnType SpawnType { get; private set; }
        public int I0 { get; private set; }
        public int I1 { get; private set; }
        public int I2 { get; private set; }
        public int I3 { get; private set; }

        public void Read(MpqFileStream stream)
        {
            this.SNOHandle = new SNOHandle(stream);
            this.SpawnType = (SpawnType)stream.ReadValueS32();
            this.I0 = stream.ReadValueS32();
            this.I1 = stream.ReadValueS32();
            this.I2 = stream.ReadValueS32();
            this.I3 = stream.ReadValueS32();
        }
    }

    public enum SpawnType : int
    {
        Normal = 0,
        Champion,
        Rare,
        Minion,
        Unique,
        Hireling,
        Clone,
    }
}
