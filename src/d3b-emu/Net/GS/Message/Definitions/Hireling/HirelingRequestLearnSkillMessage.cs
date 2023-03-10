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

namespace D3BEmu.Net.GS.Message.Definitions.Hireling
{
	/// <summary>
    /// Sent to the client when a skill is requested to be learnt for a follower
    /// </summary>
    [Message(Opcodes.HirelingRequestLearnSkillMessage, Consumers.Player)]
    public class HirelingRequestLearnSkillMessage : GameMessage
    {
        public uint HirelingID; //Actor ID of the hireling
        public int /* sno */ PowerSNOId;

        public override void Parse(GameBitBuffer buffer)
        {
            HirelingID = buffer.ReadUInt(32);
            PowerSNOId = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, HirelingID);
            buffer.WriteInt(32, PowerSNOId);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("HirelingRequestLearnSkillMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("HirelingID: 0x" + HirelingID.ToString("X8") + " (" + HirelingID + ")");
            b.Append(' ', pad); b.AppendLine("PowerSNOId: 0x" + PowerSNOId.ToString("X8"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}