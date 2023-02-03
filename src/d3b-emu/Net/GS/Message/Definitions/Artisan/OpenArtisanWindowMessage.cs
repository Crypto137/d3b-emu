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
using D3BEmu.Net.GS.Message.Definitions.Misc;

namespace D3BEmu.Net.GS.Message.Definitions.Artisan
{
    /// <summary>
    /// Shows Artisans UI window.
    /// </summary>
    [Message(Opcodes.OpenArtisanWindowMessage)]
    public class OpenArtisanWindowMessage : GameMessage
    {
        public uint ArtisanID;
        public OpenArtisanWindowMessage()
            : base(Opcodes.OpenArtisanWindowMessage)
        { }

        public override void Parse(GameBitBuffer buffer)
        {
            ArtisanID = buffer.ReadUInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ArtisanID);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("OpenArtisanWindowMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ArtisanID: 0x" + ArtisanID.ToString("X8") + " (" + ArtisanID + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }

    }
}
