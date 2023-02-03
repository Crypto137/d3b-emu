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
    /// <summary>
    /// Client -> Server
    /// 
    /// Sent when the player turns eg. while using a power
    /// </summary>
    [Message(Opcodes.PlayerTranslateFacingMessage, Consumers.Player)]
    public class PlayerTranslateFacingMessage : GameMessage
    {
        /// <summary>
        /// Id of the player actor
        /// </summary>
        public uint ActorId;

        /// <summary>
        /// Angle between actor X axis and world X axis in radians
        /// </summary>
        public float Angle;

        /// <summary>
        /// Sets whether the player turned immediatly or smoothly
        /// </summary>
        public bool TurnImmediately;

        public PlayerTranslateFacingMessage() : base(Opcodes.PlayerTranslateFacingMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorId = buffer.ReadUInt(32);
            Angle = buffer.ReadFloat32();
            TurnImmediately = buffer.ReadBool();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ActorId);
            buffer.WriteFloat32(Angle);
            buffer.WriteBool(TurnImmediately);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PlayerTranslateFacingMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorId.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("Angle: " + Angle.ToString("G"));
            b.Append(' ', pad); b.AppendLine("TurnImmediately: " + (TurnImmediately ? "true" : "false"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }

    }
}
