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

using System.Text;

namespace D3BEmu.Net.GS.Message.Definitions.Tutorial
{
    /// <summary>
    /// Sent by the client after it has shown a tutorial
    /// </summary>
    [Message(Opcodes.TutorialShownMessage, Consumers.Player)]
    public class TutorialShownMessage : GameMessage
    {
        public int SNOTutorial;

        public override void Parse(GameBitBuffer buffer)
        {
            SNOTutorial = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, SNOTutorial);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("TutorialShownMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("SNOTutorial: 0x" + SNOTutorial.ToString("X8"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}