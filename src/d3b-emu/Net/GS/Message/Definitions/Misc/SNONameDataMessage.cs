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
using D3BEmu.Core.GS.Common.Types.SNO;
using D3BEmu.Net.GS.Message.Fields;

namespace D3BEmu.Net.GS.Message.Definitions.Misc
{
    [Message(Opcodes.SNONameDataMessage)]
    public class SNONameDataMessage : GameMessage
    {
        public SNOHandle Name;

        public SNONameDataMessage() : base(Opcodes.SNONameDataMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            Name = new SNOHandle();
            Name.Parse(buffer);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            Name.Encode(buffer);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("SNONameDataMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            Name.AsText(b, pad);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
