﻿/*
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
using D3BEmu.Net.GS.Message.Fields;

namespace D3BEmu.Net.GS.Message.Definitions.ACD
{
    /// <summary>
    /// Sent by server to clients to notify about an actor movement.
    /// </summary>
    [Message(Opcodes.ACDTranslateNormalMessage)]
    public class ACDTranslateNormalMessage : GameMessage    // Formerly known as NotifyActorMovementMessage
    {
        public int ActorId;
        public Vector3D Position;           // New position of the Actor
        public float? Angle;    // Angle between actors X axis and world x axis in radians
        public bool? TurnImmediately;                // maybe immediatly rotating like in TranslateFacing? - farmy
        public float? Speed;                // Speed of the actor while moving, if moving. In game units / tick
        public int? Field5;
        public int? AnimationTag;           // Animation used while moving, if moving
        public int? Field7;
        // Later builds seem to have int? Field8 and int? Field9 in their implementation, no idea if that's supposed to be here as well

        public ACDTranslateNormalMessage():base(Opcodes.ACDTranslateNormalMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorId = buffer.ReadInt(32);
            if (buffer.ReadBool())
            {
                Position = new Vector3D();
                Position.Parse(buffer);
            }
            if (buffer.ReadBool())
            {
                Angle = buffer.ReadFloat32();
            }
            if (buffer.ReadBool())
            {
                TurnImmediately = buffer.ReadBool();
            }
            if (buffer.ReadBool())
            {
                Speed = buffer.ReadFloat32();
            }
            if (buffer.ReadBool())
            {
                Field5 = buffer.ReadInt(24);
            }
            if (buffer.ReadBool())
            {
                AnimationTag = buffer.ReadInt(21) + (-1);
            }
            if (buffer.ReadBool())
            {
                Field7 = buffer.ReadInt(32);
            }
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, ActorId);
            buffer.WriteBool(Position != null);
            if (Position != null)
            {
                Position.Encode(buffer);
            }
            buffer.WriteBool(Angle.HasValue);
            if (Angle.HasValue)
            {
                buffer.WriteFloat32(Angle.Value);
            }
            buffer.WriteBool(TurnImmediately.HasValue);
            if (TurnImmediately.HasValue)
            {
                buffer.WriteBool(TurnImmediately.Value);
            }
            buffer.WriteBool(Speed.HasValue);
            if (Speed.HasValue)
            {
                buffer.WriteFloat32(Speed.Value);
            }
            buffer.WriteBool(Field5.HasValue);
            if (Field5.HasValue)
            {
                buffer.WriteInt(24, Field5.Value);
            }
            buffer.WriteBool(AnimationTag.HasValue);
            if (AnimationTag.HasValue)
            {
                buffer.WriteInt(21, AnimationTag.Value - (-1));
            }
            buffer.WriteBool(Field7.HasValue);
            if (Field7.HasValue)
            {
                buffer.WriteInt(32, Field7.Value);
            }
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ACDTranslateNormalMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorId: 0x" + ActorId.ToString("X8"));
            if (Position != null)
            {
                Position.AsText(b, pad);
            }
            if (Angle.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("Angle.Value: " + Angle.Value.ToString("G"));
            }
            if (TurnImmediately.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("TurnImmediately.Value: " + (TurnImmediately.Value ? "true" : "false"));
            }
            if (Speed.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("Speed.Value: " + Speed.Value.ToString("G"));
            }
            if (Field5.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("Field5.Value: 0x" + Field5.Value.ToString("X8") + " (" + Field5.Value + ")");
            }
            if (AnimationTag.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("AnimationTag.Value: 0x" + AnimationTag.Value.ToString("X8") + " (" + AnimationTag.Value + ")");
            }
            if (Field7.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("Field7.Value: 0x" + Field7.Value.ToString("X8") + " (" + Field7.Value + ")");
            }
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
