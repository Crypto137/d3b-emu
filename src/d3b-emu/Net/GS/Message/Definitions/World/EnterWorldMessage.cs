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
using D3BEmu.Core.GS.Common.Types.Math;
using D3BEmu.Net.GS.Message.Fields;

namespace D3BEmu.Net.GS.Message.Definitions.World
{
    [Message(Opcodes.EnterWorldMessage)]
    public class EnterWorldMessage : GameMessage
    {
        public Vector3D EnterPosition;
        public uint WorldID; // World's DynamicID
        public int /* sno */ WorldSNO;

        public EnterWorldMessage() : base(Opcodes.EnterWorldMessage) {}

        public override void Parse(GameBitBuffer buffer)
        {
            EnterPosition = new Vector3D();
            EnterPosition.Parse(buffer);
            WorldID = buffer.ReadUInt(32);
            WorldSNO = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            EnterPosition.Encode(buffer);
            buffer.WriteUInt(32, WorldID);
            buffer.WriteInt(32, WorldSNO);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("EnterWorldMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            EnterPosition.AsText(b, pad);
            b.Append(' ', pad); b.AppendLine("WorldID: 0x" + WorldID.ToString("X8") + " (" + WorldID + ")");
            b.Append(' ', pad); b.AppendLine("WorldSNO: 0x" + WorldSNO.ToString("X8"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
