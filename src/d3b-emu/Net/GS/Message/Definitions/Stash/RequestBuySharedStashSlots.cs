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

namespace D3BEmu.Net.GS.Message.Definitions.Stash
{
    /// <summary>
    ///  Sent by the client, when the player clicks on the button to expand the size of his shared stash
    /// </summary>
    [Message(Opcodes.RequestBuySharedStashSlotsMessage, Consumers.Inventory)]
    public class RequestBuySharedStashSlotsMessage : GameMessage
    {
        public bool Field0;

        public RequestBuySharedStashSlotsMessage() : base(Opcodes.RequestBuySharedStashSlotsMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadBool();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteBool(Field0);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("RequestBuySharedStashSlots:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: " + (Field0 ? "true" : "false"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
