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
using D3.Quests;

namespace D3BEmu.Net.GS.Message.Definitions.Quest
{
    [Message(Opcodes.QuestStepCompleteMessage)]
    public class QuestStepCompleteMessage : GameMessage
    {

        public QuestStepComplete QuestStepComplete;

        public QuestStepCompleteMessage() : base(Opcodes.QuestStepCompleteMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            QuestStepComplete = QuestStepComplete.ParseFrom(buffer.ReadBlob(32));
        }
             
        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteBlob(32, QuestStepComplete.ToByteArray());
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("QuestStepCompleteMessage:");
            b.Append(' ', pad++);
            b.Append(QuestStepComplete.ToString());
        }

    }
}