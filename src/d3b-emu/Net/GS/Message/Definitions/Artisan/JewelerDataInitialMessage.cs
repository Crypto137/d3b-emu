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
using D3.ItemCrafting;

namespace D3BEmu.Net.GS.Message.Definitions.Artisan
{
    [Message(new Opcodes[] { Opcodes.JewelerDataInitialMessage })]
    public class JewelerDataInitialMessage : GameMessage
    {

        public CrafterData CrafterData;

        public JewelerDataInitialMessage() : base(Opcodes.JewelerDataInitialMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            CrafterData = CrafterData.ParseFrom(buffer.ReadBlob(32));
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteBlob(32, CrafterData.ToByteArray());
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("JewelerDataInitialMessage:");
            b.Append(' ', pad++);
            b.Append(CrafterData.ToString());
        }

    }
}