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

namespace D3BEmu.Net.GS.Message.Definitions.ACD
{
    [Message(Opcodes.ACDTranslateSyncMessage)]
    public class ACDTranslateSyncMessage : GameMessage
    {
        public int Field0;
        public Vector3D Field1;
        public bool? Field2;

        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadInt(32);
            Field1 = new Vector3D();
            Field1.Parse(buffer);
            if (buffer.ReadBool())
            {
                Field2 = buffer.ReadBool();
            }
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, Field0);
            Field1.Encode(buffer);
            buffer.WriteBool(Field2.HasValue);
            if (Field2.HasValue)
            {
                buffer.WriteBool(Field2.Value);
            }
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ACDTranslateSyncMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: 0x" + Field0.ToString("X8"));
            Field1.AsText(b, pad);
            if (Field2.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("Field2.Value: " + (Field2.Value ? "true" : "false"));
            }
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}