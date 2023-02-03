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

namespace D3BEmu.Net.GS.Message.Definitions.Player
{
    [Message(Opcodes.LoadCompleteMessage)]
    public class LoadCompleteMessage : GameMessage, ISelfHandler
    {
        public void Handle(GameClient client)
        {
            client.Player.Attributes[GameAttribute.Banter_Cooldown, 0xFFFFF] = 0x000007C9;
            client.Player.Attributes[GameAttribute.Buff_Active, 0x20CBE] = true;
            client.Player.Attributes[GameAttribute.Buff_Active, 0x33C40] = false;
            client.Player.Attributes[GameAttribute.Immobolize] = false;
            client.Player.Attributes[GameAttribute.Untargetable] = false;
            client.Player.Attributes[GameAttribute.CantStartDisplayedPowers] = false;
            client.Player.Attributes[GameAttribute.Buff_Icon_Start_Tick0, 0x20CBE] = 0xC1;
            client.Player.Attributes[GameAttribute.Disabled] = false;
            client.Player.Attributes[GameAttribute.Hidden] = false;
            client.Player.Attributes[GameAttribute.Buff_Icon_Count0, 0x33C40] = 0;
            client.Player.Attributes[GameAttribute.Buff_Icon_End_Tick0, 0x20CBE] = 0x7C9;
            client.Player.Attributes[GameAttribute.Loading] = false;
            client.Player.Attributes[GameAttribute.Buff_Icon_End_Tick0, 0x33C40] = 0;
            client.Player.Attributes[GameAttribute.Invulnerable] = false;
            client.Player.Attributes[GameAttribute.Buff_Icon_Count0, 0x20CBE] = 1;
            client.Player.Attributes[GameAttribute.Buff_Icon_Start_Tick0, 0x33C40] = 0;

            client.Player.Attributes.BroadcastChangedIfRevealed();
        }

        public override void Parse(GameBitBuffer buffer)
        {
        }

        public override void Encode(GameBitBuffer buffer)
        {
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("LoadCompleteMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}