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

using System.Collections.Generic;
using CrystalMpq;
using Gibbed.IO;
using D3BEmu.Common.MPQ.FileFormats.Types;
using D3BEmu.Core.GS.Common.Types.SNO;

namespace D3BEmu.Common.MPQ.FileFormats
{
    /// <summary>
    ///  There are no conversationLists in the CoreData with size > 0, so this is not used (yet)
    /// </summary>
    [FileFormat(SNOGroup.ConversationList)]
    public class ConversationList : FileFormat
    {
        public Header Header;
        public List<ConversationListEntry> ConversationListEntries { get; private set; }

        public ConversationList(MpqFile file)
        {
            MpqFileStream stream = file.Open();
            this.Header = new Header(stream);
            stream.Position += (12);
            ConversationListEntries = stream.ReadSerializedData<ConversationListEntry>();
            stream.Close();
        }
    }

    public class ConversationListEntry : ISerializableData
    {
        public int SNOConv { get; private set; }
        public int I0 { get; private set; }
        public int I1 { get; private set; }
        public int I2 { get; private set; }
        public int GbidItem { get; private set; }
        public string Noname1 { get; private set; }
        public string Noname2 { get; private set; }
        public int SNOQuestCurrent { get; private set; }
        public int I3 { get; private set; }
        public int SNOQuestAssigned { get; private set; }
        public int SNOQuestActive { get; private set; }
        public int SNOQuestComplete { get; private set; }
        public int SNOQuestRange { get; private set; }
        public int SNOLevelArea { get; private set; }

        public void Read(MpqFileStream stream)
        {
            SNOConv = stream.ReadValueS32();
            I0 = stream.ReadValueS32();
            I1 = stream.ReadValueS32();
            I2 = stream.ReadValueS32();
            GbidItem = stream.ReadValueS32();
            Noname1 = stream.ReadString(128, true);
            Noname2 = stream.ReadString(128, true);
            SNOQuestCurrent = stream.ReadValueS32();
            I3 = stream.ReadValueS32();
            SNOQuestAssigned = stream.ReadValueS32();
            SNOQuestActive = stream.ReadValueS32();
            SNOQuestComplete = stream.ReadValueS32();
            SNOQuestRange = stream.ReadValueS32();
            SNOLevelArea = stream.ReadValueS32();
        }
    }
}