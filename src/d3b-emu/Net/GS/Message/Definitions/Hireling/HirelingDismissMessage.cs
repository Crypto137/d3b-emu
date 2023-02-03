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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D3BEmu.Net.GS.Message.Definitions.Hireling
{
    [Message(Opcodes.HirelingDismissMessage, Consumers.Player)]
    public class HirelingDismissMessage : GameMessage
    {
        public uint HirelingId;

        public override void Parse(GameBitBuffer buffer)
        {
            HirelingId = buffer.ReadUInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, HirelingId);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("HirelingDismissMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("HirelingId: 0x" + HirelingId.ToString("X8") + " (" + HirelingId + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
