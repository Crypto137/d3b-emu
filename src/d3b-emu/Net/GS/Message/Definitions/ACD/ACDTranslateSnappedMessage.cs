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
using D3BEmu.Core.GS.Common.Types.Math;

namespace D3BEmu.Net.GS.Message.Definitions.ACD
{
    [Message(Opcodes.ACDTranslateSnappedMessage)]
    public class ACDTranslateSnappedMessage : GameMessage
    {
        public int ActorId;
        public Vector3D Position;
        public float /* angle */ Angle;
        public bool Field3;
        public int Field4;

        public ACDTranslateSnappedMessage() : base(Opcodes.ACDTranslateSnappedMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorId = buffer.ReadInt(32);
            Position = new Vector3D();
            Position.Parse(buffer);
            Angle = buffer.ReadFloat32();
            Field3 = buffer.ReadBool();
            Field4 = buffer.ReadInt(24);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, ActorId);
            Position.Encode(buffer);
            buffer.WriteFloat32(Angle);
            buffer.WriteBool(Field3);
            buffer.WriteInt(24, Field4);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ACDTranslateSnappedMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorId: 0x" + ActorId.ToString("X8"));
            Position.AsText(b, pad);
            b.Append(' ', pad); b.AppendLine("Angle: " + Angle.ToString("G"));
            b.Append(' ', pad); b.AppendLine("Field3: " + (Field3 ? "true" : "false"));
            b.Append(' ', pad); b.AppendLine("Field4: 0x" + Field4.ToString("X8") + " (" + Field4 + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}