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
using D3BEmu.Common.Helpers;
using D3BEmu.Core.GS.Skills;
using D3BEmu.Net.GS.Message.Fields;

namespace D3BEmu.Net.GS.Message.Definitions.Animation
{
    [Message(Opcodes.SecondaryAnimationPowerMessage, Consumers.Player)]
    public class SecondaryAnimationPowerMessage : GameMessage
    {
        public int /* sno */ PowerSNO;
        public AnimPreplayData Field1;

        public override void Parse(GameBitBuffer buffer)
        {
            PowerSNO = buffer.ReadInt(32);
            if (buffer.ReadBool())
            {
                Field1 = new AnimPreplayData();
                Field1.Parse(buffer);
            }
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, PowerSNO);
            buffer.WriteBool(Field1 != null);
            if (Field1 != null)
            {
                Field1.Encode(buffer);
            }
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("SecondaryAnimationPowerMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("PowerSNO: 0x" + PowerSNO.ToString("X8"));
            if (Field1 != null)
            {
                Field1.AsText(b, pad);
            }
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }

}
