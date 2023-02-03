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

using System.Text;
using D3BEmu.Net.GS.Message.Fields;

namespace D3BEmu.Net.GS.Message.Definitions.World
{
    [Message(Opcodes.TargetMessage,Consumers.Player)]
    public class TargetMessage : GameMessage
    {
        public int Type;
        public uint TargetID; // Targeted actor's DynamicID
        public WorldPlace Place;
        public int /* sno */ PowerSNO; // SNO of the power that was used on the targeted actor
        public int Field4;
        public int ComboLevel;
        public AnimPreplayData AnimPreplayData;

        public override void Parse(GameBitBuffer buffer)
        {
            Type = buffer.ReadInt(2) + (-1);
            TargetID = buffer.ReadUInt(32);
            Place = new WorldPlace();
            Place.Parse(buffer);
            PowerSNO = buffer.ReadInt(32);
            Field4 = buffer.ReadInt(32);
            ComboLevel = buffer.ReadInt(2);
            if (buffer.ReadBool())
            {
                AnimPreplayData = new AnimPreplayData();
                AnimPreplayData.Parse(buffer);
            }
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(2, Type - (-1));
            buffer.WriteUInt(32, TargetID);
            Place.Encode(buffer);
            buffer.WriteInt(32, PowerSNO);
            buffer.WriteInt(32, Field4);
            buffer.WriteInt(2, ComboLevel);
            buffer.WriteBool(AnimPreplayData != null);
            if (AnimPreplayData != null)
            {
                AnimPreplayData.Encode(buffer);
            }
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("TargetMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Type: 0x" + Type.ToString("X8") + " (" + Type + ")");
            b.Append(' ', pad); b.AppendLine("TargetID: 0x" + TargetID.ToString("X8") + " (" + TargetID + ")");
            Place.AsText(b, pad);
            b.Append(' ', pad); b.AppendLine("PowerSNO: 0x" + PowerSNO.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("Field4: 0x" + Field4.ToString("X8") + " (" + Field4 + ")");
            b.Append(' ', pad); b.AppendLine("ComboLevel: 0x" + ComboLevel.ToString("X8") + " (" + ComboLevel + ")");
            if (AnimPreplayData != null)
            {
                AnimPreplayData.AsText(b, pad);
            }
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }

}
