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

namespace D3BEmu.Net.GS.Message.Definitions.Misc
{
    /// <summary>
    /// Sent by the client if the player busy state changes. It is true if the player
    /// is currently in game menues or with an open trade window
    /// </summary>
    [Message(Opcodes.PlayerBusyMessage)]
    public class PlayerBusyMessage : GameMessage, ISelfHandler
    {
        public bool Busy;

        public override void Parse(GameBitBuffer buffer)
        {
            Busy = buffer.ReadBool();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteBool(Busy);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("BoolDataMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Busy: " + (Busy ? "true" : "false"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }

        public void Handle(GameClient client)
        {
            // TODO: PlayerBusyMessage - The status change is sent back to the client,
            // I am waiting for an autosyncing implementation of GameAttributes - farmy
            client.Player.Attributes[GameAttribute.Busy] = this.Busy;
        }
    }
}
