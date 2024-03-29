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
using D3BEmu.Core.GS.Common.Types.Math;
using D3BEmu.Core.GS.Common.Types.SNO;

namespace D3BEmu.Common.MPQ.FileFormats
{
    [FileFormat(SNOGroup.Observer)]
    public class Observer : FileFormat
    {
        public Header Header { get; private set; }
        public int I0 { get; private set; }
        public float Angle0 { get; private set; }
        public float F0 { get; private set; }
        public float F1 { get; private set; }
        public float Velocity { get; private set; }
        public float Angle1 { get; private set; }
        public float Angle2 { get; private set; }
        public float F2 { get; private set; }
        public Vector3D V0 { get; private set; }
        public Vector3D V1 { get; private set; }
        public float F3 { get; private set; }
        public float F4 { get; private set; }
        public float F5 { get; private set; }
        public float F6 { get; private set; }

        public Observer(MpqFile file)
        {
            var stream = file.Open();
            this.Header = new Header(stream);
            this.I0 = stream.ReadValueS32();
            this.Angle0 = stream.ReadValueF32();
            this.F0 = stream.ReadValueF32();
            this.F1 = stream.ReadValueF32();
            this.Velocity = stream.ReadValueF32();
            this.Angle1 = stream.ReadValueF32();
            this.Angle2 = stream.ReadValueF32();
            this.F2 = stream.ReadValueF32();
            this.V0 = new Vector3D(stream.ReadValueF32(), stream.ReadValueF32(), stream.ReadValueF32());
            this.V1 = new Vector3D(stream.ReadValueF32(), stream.ReadValueF32(), stream.ReadValueF32());
            this.F3 = stream.ReadValueF32();
            this.F4 = stream.ReadValueF32();
            this.F5 = stream.ReadValueF32();
            this.F6 = stream.ReadValueF32();
            stream.Close();
        }
    }
}