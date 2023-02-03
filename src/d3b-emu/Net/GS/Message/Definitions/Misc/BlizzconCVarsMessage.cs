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

namespace D3BEmu.Net.GS.Message.Definitions.Misc
{
    [Message(Opcodes.BlizzconCVarsMessage)]
    public class BlizzconCVarsMessage : GameMessage
    {
        public bool Field0;
        public bool Field1;
        public bool Field2;

        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadBool();
            Field1 = buffer.ReadBool();
            Field2 = buffer.ReadBool();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteBool(Field0);
            buffer.WriteBool(Field1);
            buffer.WriteBool(Field2);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("BlizzconCVarsMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: " + (Field0 ? "true" : "false"));
            b.Append(' ', pad); b.AppendLine("Field1: " + (Field1 ? "true" : "false"));
            b.Append(' ', pad); b.AppendLine("Field2: " + (Field2 ? "true" : "false"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}