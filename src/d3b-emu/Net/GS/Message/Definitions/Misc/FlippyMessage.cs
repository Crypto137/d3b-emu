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
using D3BEmu.Net.GS.Message.Fields;

namespace D3BEmu.Net.GS.Message.Definitions.Misc
{
    /// <summary>
    /// Sent to the client to play an item drop animation
    /// </summary>
    [Message(Opcodes.FlippyMessage)]
    public class FlippyMessage : GameMessage
    {
        public int ActorID;             // Effect is created at the actors location
        public int SNOParticleEffect;   // SNO for a particle effect or 0x6d82 (default_flippy) for an appearance effect
        public int SNOFlippyActor;      // -1 for a particle effect or ActorSNO for the actor to use during flipping
        public Vector3D Destination;

        public FlippyMessage() : base(Opcodes.FlippyMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorID = buffer.ReadInt(32);
            SNOParticleEffect = buffer.ReadInt(32);
            SNOFlippyActor = buffer.ReadInt(32);
            Destination = new Vector3D();
            Destination.Parse(buffer);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, ActorID);
            buffer.WriteInt(32, SNOParticleEffect);
            buffer.WriteInt(32, SNOFlippyActor);
            Destination.Encode(buffer);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("FlippyMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorID.ToString("X8") + " (" + ActorID + ")");
            b.Append(' ', pad); b.AppendLine("SNOParticleEffect: 0x" + SNOParticleEffect.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("SNOFlippyActor: 0x" + SNOFlippyActor.ToString("X8"));
            Destination.AsText(b, pad);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}