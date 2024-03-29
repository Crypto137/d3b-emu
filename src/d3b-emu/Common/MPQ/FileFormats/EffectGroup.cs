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

using CrystalMpq;
using Gibbed.IO;
using D3BEmu.Common.MPQ.FileFormats.Types;
using System.Collections.Generic;
using D3BEmu.Core.GS.Common.Types.SNO;

namespace D3BEmu.Common.MPQ.FileFormats
{
    [FileFormat(SNOGroup.EffectGroup)]
    public class EffectGroup : FileFormat
    {
        public Header Header { get; private set; }
        public int I0 { get; private set; }
        public int I1 { get; private set; }
        public int I2 { get; private set; }
        public int I3 { get; private set; }
        public int I4 { get; private set; }
        public int SnoPower { get; private set; }
        public List<EffectItem> EffectItems = new List<EffectItem>();

        public EffectGroup(MpqFile file)
        {
            var stream = file.Open();
            this.Header = new Header(stream);
            this.I0 = stream.ReadValueS32();
            this.EffectItems = stream.ReadSerializedData<EffectItem>();
            this.I1 = stream.ReadValueS32();
            stream.Position += 4; // pad 1
            this.I2 = stream.ReadValueS32();
            this.I3 = stream.ReadValueS32();
            this.I4 = stream.ReadValueS32();
            this.SnoPower = stream.ReadValueS32();
            stream.Close();
        }
    }
    public class EffectItem : ISerializableData
    {
        public int I0 { get; private set; }
        public string Name { get; private set; } // 64
        public MsgTriggeredEvent TriggeredEvent = new MsgTriggeredEvent();

        public void Read(MpqFileStream stream)
        {
            this.I0 = stream.ReadValueS32();
            // Maybe this should stay a Char Array instead of a string. - DarkLotus
            this.Name = stream.ReadString(64, true);
            this.TriggeredEvent.Read(stream);
        }
    }
   
}