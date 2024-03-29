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

namespace D3BEmu.Net.GS.Message.Definitions.ACD
{
    /// <summary>
    /// Sent to the client after a new actor is introduced with ACDEnterKnownMessage.
    /// This message adds the actor to the client's world.
    /// </summary>
    [Message(Opcodes.ACDCreateActorMessage)]
    public class ACDCreateActorMessage : GameMessage
    {
        public uint ActorId; // Actor's DynamicID

        public ACDCreateActorMessage(uint actorID)
            : base(Opcodes.ACDCreateActorMessage)
        {
            this.ActorId = actorID;
        }

        public ACDCreateActorMessage() 
            : base(Opcodes.ACDCreateActorMessage)
        { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorId = buffer.ReadUInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ActorId);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ACDCreateActorMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorId.ToString("X8") + " (" + ActorId + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}