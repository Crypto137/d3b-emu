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

namespace D3BEmu.Net.GS.Message.Definitions.Player
{
    [Message(Opcodes.PlayerChangeHotbarButtonMessage, Consumers.Player)]
    public class PlayerChangeHotbarButtonMessage : GameMessage
    {
        public int BarIndex;
        public HotbarButtonData ButtonData;

        public override void Parse(GameBitBuffer buffer)
        {
            BarIndex = buffer.ReadInt(4) + (-1);
            ButtonData = new HotbarButtonData();
            ButtonData.Parse(buffer);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(4, BarIndex - (-1));
            ButtonData.Encode(buffer);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PlayerChangeHotbarButtonMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: 0x" + BarIndex.ToString("X8") + " (" + BarIndex + ")");
            ButtonData.AsText(b, pad);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
