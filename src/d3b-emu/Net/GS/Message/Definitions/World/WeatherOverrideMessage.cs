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

namespace D3BEmu.Net.GS.Message.Definitions.World
{
    [Message(Opcodes.WeatherOverrideMessage)]
    public class WeatherOverrideMessage : GameMessage
    {
        public float Field0;
        public float Field1;

        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadFloat32();
            Field1 = buffer.ReadFloat32();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteFloat32(Field0);
            buffer.WriteFloat32(Field1);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("WeatherOverrideMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: " + Field0.ToString("G"));
            b.Append(' ', pad); b.AppendLine("Field1: " + Field1.ToString("G"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}