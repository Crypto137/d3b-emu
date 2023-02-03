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

namespace D3BEmu.Net.GS.Message.Definitions.Connection
{
    [Message(new[] { Opcodes.LogoutContextMessage1, Opcodes.LogoutContextMessage2 })]
    public class LogoutContextMessage : GameMessage,ISelfHandler
    {
        public bool Field0;

        public void Handle(GameClient client)
        {
            client.IsLoggingOut = !client.IsLoggingOut;

            if (client.IsLoggingOut)
            {
                client.SendMessage(new LogoutTickTimeMessage()
                {
                    Id = 0x0027,
                    Field0 = false, // true - logout with party?
                    Ticks = 0, // delay 1, make this equal to 0 for instant logout
                    Field2 = 0, // delay 2
                });
            }
        }

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
            b.AppendLine("LogoutContextMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: " + (Field0 ? "true" : "false"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
