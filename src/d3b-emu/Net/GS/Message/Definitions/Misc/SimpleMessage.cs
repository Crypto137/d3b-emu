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
using System.Text;

namespace D3BEmu.Net.GS.Message.Definitions.Misc
{
    [Message(new[]{
        Opcodes.SimpleMessage1, Opcodes.SimpleMessage2, Opcodes.SimpleMessage5, Opcodes.SimpleMessage6, Opcodes.GameSetupMessageAck, Opcodes.SimpleMessage8,
        Opcodes.SimpleMessage9, Opcodes.SimpleMessage10, Opcodes.SimpleMessage13, Opcodes.SimpleMessage14, Opcodes.SimpleMessage15, Opcodes.SimpleMessage16, 
        Opcodes.SimpleMessage18, Opcodes.SimpleMessage19, Opcodes.SimpleMessage20, Opcodes.SimpleMessage21, Opcodes.SimpleMessage22, Opcodes.SimpleMessage23, Opcodes.SimpleMessage24, 
        Opcodes.SimpleMessage25, Opcodes.SimpleMessage26, Opcodes.SimpleMessage27, Opcodes.SimpleMessage28, Opcodes.SimpleMessage29, Opcodes.SimpleMessage30, Opcodes.SimpleMessage31,
        Opcodes.SimpleMessage32, Opcodes.SimpleMessage34, Opcodes.SimpleMessage35, Opcodes.SimpleMessage36, Opcodes.SimpleMessage37, Opcodes.SimpleMessage38, 
        Opcodes.SimpleMessage39, Opcodes.SimpleMessage40, Opcodes.NPCInteractCancel, Opcodes.SimpleMessage46
    })]
    public class SimpleMessage : GameMessage, ISelfHandler
    {
        public void Handle(GameClient client)
        {
            // do not return back a not-implemented exception! /raist.
        }

        public override void Parse(GameBitBuffer buffer)
        {
        }

        public override void Encode(GameBitBuffer buffer)
        {
            throw new NotImplementedException();
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("SimpleMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
