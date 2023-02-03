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
using D3BEmu.Core.GS.Map;

namespace D3BEmu.Net.GS.Message.Definitions.Map
{
    [Message(Opcodes.MapRevealSceneMessage)]
    public class MapRevealSceneMessage : GameMessage
    {
        public uint ChunkID;
        public int /* sno */ SceneSNO;
        public PRTransform Transform;
        public uint WorldID;
        public SceneMiniMapVisibility MiniMapVisibility;

        public MapRevealSceneMessage() : base(Opcodes.MapRevealSceneMessage) {}

        public override void Parse(GameBitBuffer buffer)
        {
            ChunkID = buffer.ReadUInt(32);
            SceneSNO = buffer.ReadInt(32);
            Transform = new PRTransform();
            Transform.Parse(buffer);
            WorldID = buffer.ReadUInt(32);
            MiniMapVisibility = (SceneMiniMapVisibility)buffer.ReadInt(3);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ChunkID);
            buffer.WriteInt(32, SceneSNO);
            Transform.Encode(buffer);
            buffer.WriteUInt(32, WorldID);
            buffer.WriteInt(3, (int)MiniMapVisibility);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("MapRevealSceneMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ChunkID: 0x" + ChunkID.ToString("X8") + " (" + ChunkID + ")");
            b.Append(' ', pad); b.AppendLine("SceneSNO: 0x" + SceneSNO.ToString("X8"));
            Transform.AsText(b, pad);
            b.Append(' ', pad); b.AppendLine("WorldID: 0x" + WorldID.ToString("X8") + " (" + WorldID + ")");
            b.Append(' ', pad); b.AppendLine("MiniMapVisibility: 0x" + ((int)MiniMapVisibility).ToString("X8") + " (" + MiniMapVisibility + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
