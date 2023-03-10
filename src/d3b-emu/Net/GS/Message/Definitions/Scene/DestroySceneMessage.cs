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

namespace D3BEmu.Net.GS.Message.Definitions.Scene
{
    [Message(Opcodes.DestroySceneMessage)]
    public class DestroySceneMessage : GameMessage
    {
        public uint WorldID;
        public uint SceneID;

        public DestroySceneMessage() : base(Opcodes.DestroySceneMessage) {}

        public override void Parse(GameBitBuffer buffer)
        {
            WorldID = buffer.ReadUInt(32);
            SceneID = buffer.ReadUInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, WorldID);
            buffer.WriteUInt(32, SceneID);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("DestroySceneMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("WorldID: 0x" + WorldID.ToString("X8") + " (" + WorldID + ")");
            b.Append(' ', pad); b.AppendLine("SceneID: 0x" + SceneID.ToString("X8") + " (" + SceneID + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
