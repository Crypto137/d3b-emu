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

namespace D3BEmu.Net.GS.Message.Definitions.Misc
{
    [Message(Opcodes.EntityIdMessage)]
    public class EntityIdMessage : GameMessage
    {
        public EntityId Field0;

        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = new EntityId();
            Field0.Parse(buffer);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            Field0.Encode(buffer);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("EntityIdMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            Field0.AsText(b, pad);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}