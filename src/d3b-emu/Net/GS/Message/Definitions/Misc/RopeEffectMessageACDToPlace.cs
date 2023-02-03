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

namespace D3BEmu.Net.GS.Message.Definitions.Misc
{
    [Message(Opcodes.RopeEffectMessageACDToPlace)]
    public class RopeEffectMessageACDToPlace : GameMessage
    {
        public int /* sno */ RopeSNO;
        public int StartSourceActorId;
        public int Field2;  // always seem to be 4
        public WorldPlace EndPosition;

        public RopeEffectMessageACDToPlace() : base(Opcodes.RopeEffectMessageACDToPlace) { }

        public override void Parse(GameBitBuffer buffer)
        {
            RopeSNO = buffer.ReadInt(32);
            StartSourceActorId = buffer.ReadInt(32);
            Field2 = buffer.ReadInt(5);
            EndPosition = new WorldPlace();
            EndPosition.Parse(buffer);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, RopeSNO);
            buffer.WriteInt(32, StartSourceActorId);
            buffer.WriteInt(5, Field2);
            EndPosition.Encode(buffer);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("RopeEffectMessageACDToPlace:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("RopeSNO: 0x" + RopeSNO.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("StartSourceActorId: 0x" + StartSourceActorId.ToString("X8") + " (" + StartSourceActorId + ")");
            b.Append(' ', pad); b.AppendLine("Field2: 0x" + Field2.ToString("X8") + " (" + Field2 + ")");
            EndPosition.AsText(b, pad);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}