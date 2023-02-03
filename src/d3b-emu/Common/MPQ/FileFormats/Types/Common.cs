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
using D3BEmu.Core.GS.Common.Types.Misc;
using D3BEmu.Core.GS.Common.Types.SNO;

namespace D3BEmu.Common.MPQ.FileFormats.Types
{
    public class Header
    {
        public int DeadBeef { get; private set; }
        public int SnoType { get; private set; }
        public int Unknown1 { get; private set; }
        public int Unknown2 { get; private set; }
        public int Unknown3 { get; private set; }
        public int Unknown4 { get; private set; }
        public int SNOId { get; private set; }

        public Header(MpqFileStream stream)
        {
            this.DeadBeef = stream.ReadValueS32();
            this.SnoType = stream.ReadValueS32();
            this.Unknown1 = stream.ReadValueS32();
            this.Unknown2 = stream.ReadValueS32();
            this.SNOId = stream.ReadValueS32();
            this.Unknown3 = stream.ReadValueS32();
            this.Unknown4 = stream.ReadValueS32();
        }
    }


    public class ScriptFormula
    {
        public int I0 { get; private set; }
        public int I1 { get; private set; }
        public int I2 { get; private set; }
        public int I3 { get; private set; }
        public int I4 { get; private set; }
        public int NameSize { get; private set; }
        public int I5 { get; private set; }
        public int OpcodeSize { get; private set; }
        public byte[] OpCodeName { get; private set; }
        public byte[] OpCodeArray { get; private set; }

        public ScriptFormula(MpqFileStream stream)
        {
            this.I0 = stream.ReadValueS32();
            this.I1 = stream.ReadValueS32();
            this.I2 = stream.ReadValueS32();
            this.I3 = stream.ReadValueS32();
            this.I4 = stream.ReadValueS32();
            this.NameSize = stream.ReadValueS32();
            this.I5 = stream.ReadValueS32();
            this.OpcodeSize = stream.ReadValueS32();
            this.OpCodeName = new byte[NameSize];
            stream.Read(OpCodeName, 0, NameSize);
            switch(NameSize % 4)
            {
                case 0:
                    break;
                case 1:
                    stream.Position += 3;
                    break;
                case 2:
                    stream.Position += 2;
                    break;
                case 3:
                    stream.Position += 1;
                    break;

            }
            this.OpCodeArray = new byte[OpcodeSize];
            stream.Read(OpCodeArray, 0, OpcodeSize);
        }
    }

    public class ScriptFormulaDetails : ISerializableData
    {
        public string CharArray1 { get; private set; }
        public string CharArray2 { get; private set; }
        public int I0 { get; private set; }
        public int I1 { get; private set; }

        public void Read(MpqFileStream stream)
        {
            CharArray1 = stream.ReadString(256, true);
            CharArray2 = stream.ReadString(512, true);
            I0 = stream.ReadValueS32();
            I1 = stream.ReadValueS32();
        }
    }

    // Replace each Look with just a chararay? DarkLotus
    public class HardPointLink
    {
        public string Name { get; private set; }
        public int I0 { get; private set; }

        public HardPointLink(MpqFileStream stream)
        {
            this.Name = stream.ReadString(64, true);
            I0 = stream.ReadValueS32();
        }
    }
    
    public class TriggerConditions
    {
        public int Percent { get; private set; } //0-255
        public int Int1 { get; private set; }
        public int Int2 { get; private set; }
        public int Int3 { get; private set; }
        public int Int4 { get; private set; }
        public int Int5 { get; private set; }
        public int Int6 { get; private set; }
        public int Int7 { get; private set; }
        public int Int8 { get; private set; }

        public TriggerConditions(MpqFileStream stream)
        {
            Percent = stream.ReadByte();
            stream.Position += 3;
            Int1 = stream.ReadValueS32();
            Int2 = stream.ReadValueS32();
            Int3 = stream.ReadValueS32();
            Int4 = stream.ReadValueS32();
            Int5 = stream.ReadValueS32();
            Int6 = stream.ReadValueS32();
            Int7 = stream.ReadValueS32();
            Int8 = stream.ReadValueS32();
        }
    }

    public class TriggerEvent
    {
        public int I0 { get; private set; }
        public TriggerConditions TriggerConditions { get; private set; }
        public int I1 { get; private set; }
        public SNOHandle SNOHandle { get; private set; }
        public int I2 { get; private set; }
        public int I3 { get; private set; }
        public int RuneType { get; private set; }
        public int UseRuneType { get; private set; }
        public HardPointLink[] HardPointLinks { get; private set; }
        public string LookLink { get; private set; }
        public string ConstraintLink { get; private set; }
        public int I4 { get; private set; }
        public float F0 { get; private set; }
        public int I5 { get; private set; }
        public int I6 { get; private set; }
        public int I7 { get; private set; }
        public int I8 { get; private set; }
        public int I9 { get; private set; }
        public float F1 { get; private set; }
        public float F2 { get; private set; }
        public int I10 { get; private set; }
        public float F3 { get; private set; }
        public int I11 { get; private set; }
        public float Velocity { get; private set; }
        public int I12 { get; private set; }
        public int Ticks1 { get; private set; } // DT_TIME
        public RGBAColor Color1 { get; private set; }
        public int I14 { get; private set; } // DT_TIME
        public RGBAColor Color2 { get; private set; }
        public int I15 { get; private set; } // DT_TIME

        public TriggerEvent(MpqFileStream stream)
        {
            I0 = stream.ReadValueS32();
            TriggerConditions = new TriggerConditions(stream);
            I1 = stream.ReadValueS32();
            SNOHandle = new SNOHandle(stream);
            I2 = stream.ReadValueS32();
            I3 = stream.ReadValueS32();
            RuneType = stream.ReadValueS32();
            UseRuneType = stream.ReadValueS32();
            HardPointLinks = new HardPointLink[2];
            HardPointLinks[0] = new HardPointLink(stream);
            HardPointLinks[1] = new HardPointLink(stream);
            this.LookLink = stream.ReadString(64, true);
            this.ConstraintLink = stream.ReadString(64, true);
            I4 = stream.ReadValueS32();
            F0 = stream.ReadValueF32();
            I5 = stream.ReadValueS32();
            I6 = stream.ReadValueS32();
            I7 = stream.ReadValueS32();
            I8 = stream.ReadValueS32();
            I9 = stream.ReadValueS32();
            F1 = stream.ReadValueF32();
            F2 = stream.ReadValueF32();
            I10 = stream.ReadValueS32();
            F3 = stream.ReadValueF32();
            I11 = stream.ReadValueS32();
            Velocity = stream.ReadValueF32();
            I12 = stream.ReadValueS32();
            Ticks1 = stream.ReadValueS32();
            Color1 = new RGBAColor(stream);
            I14 = stream.ReadValueS32();
            Color2 = new RGBAColor(stream);
            I15 = stream.ReadValueS32();
        }
    }

    public class MsgTriggeredEvent : ISerializableData
    {
        public int I0 { get; private set; }
        public TriggerEvent TriggerEvent { get; private set; }

        public void Read(MpqFileStream stream)
        {
            I0 = stream.ReadValueS32();
            TriggerEvent = new TriggerEvent(stream);
        }
    }


    public class ItemSpecifierData
    {
        public int ItemGBId { get; private set; }
        public int I0 { get; private set; }
        public int[] GBIdAffixes = new int[3];
        public int I1 { get; private set; }

        public ItemSpecifierData(MpqFileStream stream)
        {
            ItemGBId = stream.ReadValueS32();
            I0 = stream.ReadValueS32();
            for (int i = 0; i < GBIdAffixes.Length; i++)
            {
                GBIdAffixes[i] = stream.ReadValueS32();
            }
            I1 = stream.ReadValueS32();
        }
    }

    //public class PostFXParams // unused for now. /raist.
    //{
    //    public float[] Float0;
    //    public float[] Float1;

    //    public PostFXParams(MpqFileStream stream)
    //    {
    //        Float0 = new float[4];
    //        for (int i = 0; i < Float0.Length; i++)
    //        {
    //            Float0[i] = stream.ReadInt32();
    //        }
    //        Float1 = new float[4];
    //        for (int i = 0; i < Float1.Length; i++)
    //        {
    //            Float1[i] = stream.ReadInt32();
    //        }
    //    }
    //}
}
