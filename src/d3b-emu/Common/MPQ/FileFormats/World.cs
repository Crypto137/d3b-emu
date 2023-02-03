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

using System.Collections.Generic;
using CrystalMpq;
using Gibbed.IO;
using D3BEmu.Common.MPQ.FileFormats.Types;
using D3BEmu.Core.GS.Common.Types.Math;
using D3BEmu.Core.GS.Common.Types.SNO;
using D3BEmu.Core.GS.Common.Types.Scene;
using D3BEmu.Core.GS.Common.Types.TagMap;

namespace D3BEmu.Common.MPQ.FileFormats
{
    [FileFormat(SNOGroup.Worlds)]
    public class World : FileFormat
    {
        public Header Header { get; private set; }

        public List<DRLGParams> DRLGParams { get; private set; }
        public SceneParams SceneParams = new SceneParams();
        public List<int> MarkerSets = new List<int>();
        public Environment Environment { get; private set; }
        public LabelRuleSet LabelRuleSet { get; private set; }        
        public SceneClusterSet SceneClusterSet { get; private set; }
        public int[] SNONavMeshFunctions = new int[4];
        public int Int0 { get; private set; }
        public float Float0 { get; private set; }
        public int Int1 { get; private set; }
        public int SNOScript { get; private set; }
        public int Int2 { get; private set; }

        public World(MpqFile file)
        {
            var stream = file.Open();

            this.Header = new Header(stream);

            this.DRLGParams = stream.ReadSerializedData<DRLGParams>(); // I'm not sure if we can have a list of drlgparams. (then should be calling it with pointer.Size/120) /raist

            stream.Position += (3 * 4);
            this.SceneParams = stream.ReadSerializedItem<SceneParams>(); // I'm not sure if we can have a list of drlgparams. (then should be calling it with pointer.Size/24) /raist

            stream.Position += (2 * 4);
            this.MarkerSets = stream.ReadSerializedInts();

            stream.Position += (14 * 4);
            this.Environment = new Environment(stream);

            stream.Position += 4;
            LabelRuleSet = new LabelRuleSet(stream);
            this.Int0 = stream.ReadValueS32();

            stream.Position += 4;
            this.SceneClusterSet = new SceneClusterSet(stream);

            for (int i = 0; i < SNONavMeshFunctions.Length; i++)
            {
                SNONavMeshFunctions[i] = stream.ReadValueS32();
            }

            stream.Position += 4;
            Float0 = stream.ReadValueF32();
            Int1 = stream.ReadValueS32();
            SNOScript = stream.ReadValueS32();
            Int2 = stream.ReadValueS32();

            stream.Close();
        }
    }

    #region scene-params

    public class SceneParams : ISerializableData
    {
        public List<SceneChunk> SceneChunks = new List<SceneChunk>();
        public int ChunkCount { get; set; }

        public void Read(MpqFileStream stream)
        {
            var pointer = stream.GetSerializedDataPointer();
            this.ChunkCount = stream.ReadValueS32();
            stream.Position += (3 * 4);
            this.SceneChunks = stream.ReadSerializedData<SceneChunk>(pointer, this.ChunkCount);
        }
    }

    public class SceneChunk : ISerializableData
    {
        public SNOHandle SNOHandle { get; set; }
        public PRTransform PRTransform { get; set; }
        public SceneSpecification SceneSpecification { get; set; }

        public void Read(MpqFileStream stream)
        {
            this.SNOHandle = new SNOHandle(stream);
            this.PRTransform = new PRTransform(stream);
            this.SceneSpecification = new SceneSpecification(stream);
        }
    }
  
    #endregion

    #region drlg-params

    public class DRLGParams : ISerializableData
    {
        public List<TileInfo> Tiles = new List<TileInfo>();
        public int CommandCount { get; private set; }
        public List<DRLGCommand> Commands = new List<DRLGCommand>();
        public List<int> ParentIndices = new List<int>();
        public TagMap TagMap { get; private set; }

        public void Read(MpqFileStream stream)
        {
            Tiles = stream.ReadSerializedData<TileInfo>();

            stream.Position += (14 * 4);
            this.CommandCount = stream.ReadValueS32();
            this.Commands = stream.ReadSerializedData<DRLGCommand>();

            stream.Position += (3 * 4);
            this.ParentIndices = stream.ReadSerializedInts();

            stream.Position += (2 * 4);
            this.TagMap = stream.ReadSerializedItem<TagMap>();
            stream.Position += (2 * 4);
        }
    }

    public enum TileExits
    {
        West = 1,
        East = 2,
        North = 4,
        South = 8,
    }

    public enum TileTypes
    {
        Normal = 100,
        EventTile1 = 101, // Jar of souls? more? Deadend?
        EventTile2 = 102, // 1000 dead
        Entrance = 200,
        UEntrance1 = 201, // Defiled crypt what there?
        Exit = 300,
        Filler = 401
    }

    public class TileInfo : ISerializableData
    {
        public int ExitDirectionBits { get; private set; }
        public int TileType { get; private set; }
        public int SNOScene { get; private set; }
        public int Probability { get; private set; }
        public TagMap TagMap { get; private set; }
        public CustomTileInfo CustomTileInfo { get; private set; }

        public void Read(MpqFileStream stream)
        {
            ExitDirectionBits = stream.ReadValueS32();
            TileType = stream.ReadValueS32();
            SNOScene = stream.ReadValueS32();
            Probability = stream.ReadValueS32();
            this.TagMap = stream.ReadSerializedItem<TagMap>();

            stream.Position += (2 * 4);
            CustomTileInfo = new CustomTileInfo(stream);
        }
    }

    public enum CommandType
    {
        Waypoint = 0,
        BridleEntrance = 1,
        AddExit = 2,
        AddHub = 3,
        AddSpoke = 4,
        Group = 9, //used in DRLG to group tiles together
    }

    public class DRLGCommand : ISerializableData
    {
        public string Name { get; private set; }
        public int CommandType { get; private set; }
        public TagMap TagMap { get; private set; }

        public void Read(MpqFileStream stream)
        {
            this.Name = stream.ReadString(128, true);
            CommandType = stream.ReadValueS32();
            this.TagMap = stream.ReadSerializedItem<TagMap>();
            stream.Position += (3 * 4);
        }
    }

    public class CustomTileInfo
    {
        public int Int0 { get; private set; }
        public int Int1 { get; private set; }
        public int Int2 { get; private set; }
        public Vector2D V0 { get; private set; }
        public List<CustomTileCell> CustomTileCells { get; private set; }

        public CustomTileInfo(MpqFileStream stream)
        {
            Int0 = stream.ReadValueS32();
            Int1 = stream.ReadValueS32();
            Int2 = stream.ReadValueS32();
            V0 = new Vector2D(stream);
            CustomTileCells = stream.ReadSerializedData<CustomTileCell>();
            stream.Position += (3 * 4);
        }
    }

    public class CustomTileCell : ISerializableData // we're not using this yet. /raist.
    {
        public int Int0;
        public int Int1;
        public int Int2;
        public int SNOScene;
        public int Int3;
        public int[] Int4;

        public CustomTileCell() { }

        public void Read(MpqFileStream stream)
        {
            Int0 = stream.ReadValueS32();
            Int1 = stream.ReadValueS32();
            Int2 = stream.ReadValueS32();
            SNOScene = stream.ReadValueS32();
            Int3 = stream.ReadValueS32();
            Int4 = new int[4];
            for (int i = 0; i < Int4.Length; i++)
            {
                Int4[i] = stream.ReadValueS32();
            }
        }
    }

    #endregion

    #region scene-cluster

    public class SceneClusterSet
    {
        public int ClusterCount { get; private set; }
        public List<SceneCluster> SceneClusters = new List<SceneCluster>();

        public SceneClusterSet(MpqFileStream stream)
        {
            this.ClusterCount = stream.ReadValueS32();
            stream.Position += (4 * 3);
            this.SceneClusters = stream.ReadSerializedData<SceneCluster>(this.ClusterCount);
        }
    }

    public class SceneCluster : ISerializableData
    {
        public string Name { get; private set; }
        public int ClusterId { get; private set; }
        public int GroupCount { get; private set; }
        public List<SubSceneGroup> SubSceneGroups = new List<SubSceneGroup>();
        public SubSceneGroup Default { get; private set; }

        public void Read(MpqFileStream stream)
        {
            this.Name = stream.ReadString(128, true);
            this.ClusterId = stream.ReadValueS32();
            this.GroupCount = stream.ReadValueS32();
            stream.Position += (2 * 4);
            this.SubSceneGroups = stream.ReadSerializedData<SubSceneGroup>(this.GroupCount);

            this.Default = new SubSceneGroup(stream);
        }
    }

    public class SubSceneGroup : ISerializableData
    {
        public int I0 { get; private set; }
        public int SubSceneCount { get; private set; }
        public List<SubSceneEntry> Entries = new List<SubSceneEntry>();

        public SubSceneGroup() { }

        public SubSceneGroup(MpqFileStream stream)
        {
            this.Read(stream);
        }

        public void Read(MpqFileStream stream)
        {
            this.I0 = stream.ReadValueS32();
            this.SubSceneCount = stream.ReadValueS32();
            stream.Position += (2 * 4);
            this.Entries = stream.ReadSerializedData<SubSceneEntry>(this.SubSceneCount);
        }
    }

    public class SubSceneEntry : ISerializableData
    {
        public int SNOScene { get; private set; }
        public int Probability { get; private set; }
        public int LabelCount { get; private set; }
        public List<SubSceneLabel> Labels = new List<SubSceneLabel>();

        public void Read(MpqFileStream stream)
        {
            this.SNOScene = stream.ReadValueS32();
            this.Probability = stream.ReadValueS32();
            stream.Position += (3 * 4);
            this.LabelCount = stream.ReadValueS32();
            this.Labels = stream.ReadSerializedData<SubSceneLabel>(this.LabelCount);
        }
    }

    public class SubSceneLabel : ISerializableData
    {
        public int GBId { get; private set; }
        public int I0 { get; private set; }

        public void Read(MpqFileStream stream)
        {
            GBId = stream.ReadValueS32();
            I0 = stream.ReadValueS32();
        }
    }

    #endregion

    #region others

    public class LabelRuleSet
    {
        public int Rulecount { get; private set; }
        public List<LabelRule> LabelRules = new List<LabelRule>();

        public LabelRuleSet(MpqFileStream stream)
        {
            Rulecount = stream.ReadValueS32();
            stream.Position += (3 * 4);
            this.LabelRules = stream.ReadSerializedData<LabelRule>(this.Rulecount);
        }
    }

    public class LabelRule : ISerializableData
    {
        public string Name { get; private set; }
        public LabelCondition LabelCondition { get; private set; }
        public int Int0 { get; private set; }
        public int LabelCount { get; private set; }
        public List<LabelEntry> Entries = new List<LabelEntry>();

        public void Read(MpqFileStream stream)
        {
            this.Name = stream.ReadString(128, true);
            LabelCondition = new LabelCondition(stream);
            Int0 = stream.ReadValueS32();
            LabelCount = stream.ReadValueS32();
            stream.Position += (2 * 4);
            this.Entries = stream.ReadSerializedData<LabelEntry>(this.LabelCount);
        }
    }

    public class LabelEntry : ISerializableData
    {
        public int GBIdLabel { get; private set; }
        public int Int0 { get; private set; }
        public float Float0 { get; private set; }
        public int Int1 { get; private set; }
        public int Int2 { get; private set; }

        public void Read(MpqFileStream stream)
        {
            this.GBIdLabel = stream.ReadValueS32();
            Int0 = stream.ReadValueS32();
            Float0 = stream.ReadValueF32();
            Int1 = stream.ReadValueS32();
            Int2 = stream.ReadValueS32();
        }
    }

    public class LabelCondition
    {
        public DT_ENUM0 Enum0 { get; private set; }
        public int Int0 { get; private set; }
        public int[] Int1 { get; private set; }

        public LabelCondition(MpqFileStream stream)
        {
            Enum0 = (DT_ENUM0)stream.ReadValueS32();
            Int0 = stream.ReadValueS32();
            Int1 = new int[4];

            for (int i = 0; i < Int1.Length; i++)
            {
                Int1[i] = stream.ReadValueS32();
            }
        }
    }

    public enum DT_ENUM0
    {
        Always = 0,
        GameDifficulty = 1,
        LabelAlreadySet = 2,
    }

    public class Environment
    {
        /*public RGBAColor RGBAColor0;
        public PostFXParams PostFXParams1;
        public int int2;
        public int int3;
        public UberMaterial UberMaterial4;
        */
        public int[] Unknown { get; private set; }
        public int snoMusic { get; private set; }
        public int snoCombatMusic { get; private set; }
        public int snoAmbient { get; private set; }
        public int snoReverb { get; private set; }
        public int snoWeather { get; private set; }
        public int snoIrradianceTex { get; private set; }
        public int snoIrradianceTexDead { get; private set; }

        public Environment(MpqFileStream stream)
        {
            Unknown = new int[38];
            for (int i = 0; i < 38; i++)
            {
                Unknown[i] = stream.ReadValueS32();
            }

            /* RGBAColor0 = new RGBAColor(stream);
             PostFXParams1 = new PostFXParams(stream);
             int2 = stream.ReadInt32();
             int3 = stream.ReadInt32();
             UberMaterial4 = new UberMaterial(stream);
             * */
             snoMusic = stream.ReadValueS32();
             snoCombatMusic = stream.ReadValueS32();
             snoAmbient = stream.ReadValueS32();
             snoReverb = stream.ReadValueS32();
             snoWeather = stream.ReadValueS32();
             snoIrradianceTex = stream.ReadValueS32();
             snoIrradianceTexDead = stream.ReadValueS32();
        }
    }

    #endregion
}