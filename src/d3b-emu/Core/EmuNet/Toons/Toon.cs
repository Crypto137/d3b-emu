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

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using D3BEmu.Common.Helpers;
using D3BEmu.Common.Helpers.Hash;
using D3BEmu.Common.MPQ;
using D3BEmu.Common.MPQ.FileFormats;
using D3BEmu.Common.Storage;
using D3BEmu.Core.GS.Players;
using D3BEmu.Core.GS.Skills;
using D3BEmu.Core.EmuNet.Accounts;
using D3BEmu.Core.EmuNet.Helpers;
using D3BEmu.Core.EmuNet.Objects;
using D3BEmu.Net.EmuNet;

namespace D3BEmu.Core.EmuNet.Toons
{
    public class Toon : PersistentRPCObject
    {
        /// <summary>
        /// D3 EntityID encoded id.
        /// </summary>
        public D3.OnlineService.EntityId D3EntityID { get; private set; }

        /// <summary>
        /// Bnet EntityID encoded id.
        /// </summary>
        public bnet.protocol.EntityId BnetEntityID { get; private set; }

        /// <summary>
        /// Toon handle struct.
        /// </summary>
        public ToonHandleHelper ToonHandle { get; private set; }

        public string Name { get; private set; }
        public int HashCode { get; set; }
        public uint TimePlayed { get; set; }
        public uint LoginTime { get; set; }
        public string HashCodeString { get; private set; }
        public ToonClass Class { get; private set; }
        public ToonFlags Flags { get; private set; }
        public byte Level { get; private set; }
        public int ExperienceNext { get; set; }

        public int? ActiveHireling { get; set; }
        public int CurrentQuest { get; set; }
        public int CurrentQuestStepId { get; set; }

        public bool UnlockedStoneOfRecall { get; set; }
        public bool UnlockedCauldronOfJordan { get; set; }
        public bool UnlockedNephalemCube { get; set; }

        public ToonSkillSet SkillSet { get; private set; }

        public byte[] Lore { get; set; }

        public D3.Hero.Digest Digest
        {
            get
            {
                return D3.Hero.Digest.CreateBuilder().SetVersion(891)
                                .SetHeroId(this.D3EntityID)
                                .SetHeroName(this.Name)
                                .SetGbidClass((int)this.ClassID)
                                .SetPlayerFlags((uint)this.Flags)
                                .SetLevel(this.Level)
                                .SetVisualEquipment(this.Equipment)
                                .SetLastPlayedAct(0)
                                .SetHighestUnlockedAct(0)
                                .SetLastPlayedDifficulty(0)
                                .SetHighestUnlockedDifficulty(0)
                                .SetLastPlayedQuest(this.CurrentQuest)
                                .SetLastPlayedQuestStep(this.CurrentQuestStepId)
                                .SetTimePlayed(this.TimePlayed)

                                .AddQuestHistory(new D3.Hero.QuestHistoryEntry.Builder().SetDifficulty(0).SetSnoQuest(GS.Games.Quest.ProtectorOfTristram))
                                .AddQuestHistory(new D3.Hero.QuestHistoryEntry.Builder().SetDifficulty(0).SetSnoQuest(GS.Games.Quest.RescueCain))
                                .AddQuestHistory(new D3.Hero.QuestHistoryEntry.Builder().SetDifficulty(0).SetSnoQuest(GS.Games.Quest.Blacksmith))
                                .AddQuestHistory(new D3.Hero.QuestHistoryEntry.Builder().SetDifficulty(0).SetSnoQuest(GS.Games.Quest.SkeletonKing))
                                .Build();
            }
        }
        public D3.Hero.VisualEquipment Equipment { get; protected set; }
        public AwayStatus AwayStatus { get; private set; }

        public Account Owner { get; set; }

        public bool IsSelected
        {
            get
            {
                if (!this.Owner.IsOnline) return false;
                else
                {
                    if (this.Owner.LoggedInClient.CurrentToon != null)
                        return this.Owner.LoggedInClient.CurrentToon.BnetEntityID == this.BnetEntityID;
                    else
                        return false;
                }
            }
        }

        public Toon(ulong persistentId, string name, int hashCode, byte @class, byte gender, byte level, int experience, long accountId, uint timePlayed,     // Toon with given persistent ID
            int currentQuest, int currentQuestStepId,
            bool unlockedStoneOfRecall, bool unlockedCauldronOfJordan, bool unlockedNephalemCube, ToonSkillSet skillSet) 
            : base(persistentId)
        {
            this.SetFields(name, hashCode, (ToonClass)@class, (ToonFlags)gender, level, experience, AccountManager.GetAccountByPersistentID((ulong)accountId), timePlayed,
                currentQuest, currentQuestStepId,
                unlockedStoneOfRecall, unlockedCauldronOfJordan, unlockedNephalemCube, skillSet);
        }

        public Toon(string name, int hashCode, int classId, ToonFlags flags, byte level, Account account) // Toon with **newly generated** persistent ID
            : base(StringHashHelper.HashIdentity(name + "#" + hashCode.ToString("D3")))
        {
            this.SetFields(name, hashCode, GetClassByID(classId), flags, level, account, 0);
        }

        public int ClassID
        {
            get
            {
                switch (this.Class)
                {
                    case ToonClass.Barbarian:
                        return 0x4FB91EE2;
                    case ToonClass.DemonHunter:
                        return unchecked((int)0xC88B9649);
                    case ToonClass.Monk:
                        return 0x3DAC15;
                    case ToonClass.WitchDoctor:
                        return 0x343C22A;
                    case ToonClass.Wizard:
                        return 0x1D4681B1;
                }
                return 0x0;
            }
        }

        public int VoiceClassID // Used for Conversations
        {
            get
            {
                switch (this.Class)
                {
                    case ToonClass.DemonHunter:
                        return 0;
                    case ToonClass.Barbarian:
                        return 1;
                    case ToonClass.Wizard:
                        return 2;
                    case ToonClass.WitchDoctor:
                        return 3;
                    case ToonClass.Monk:
                        return 4;
                }
                return 0;
            }
        }

        public int Gender
        {
            get
            {
                return (int)(this.Flags & ToonFlags.Female); // 0x00 for male, so we can just return the AND operation
            }
        }

        #region c-tor and setfields

        public HeroTable HeroTable { get; private set; }
        private static readonly GameBalance HeroData = (GameBalance)MPQStorage.Data.Assets[GS.Common.Types.SNO.SNOGroup.GameBalance][19740].Data;

        private void SetFields(string name, int hashCode, ToonClass @class, ToonFlags flags, byte level, int experience, Account owner, uint timePlayed,
            int currentQuest, int currentQuestStepId,
            bool unlockedStoneOfRecall, bool unlockedCauldronOfJordan, bool unlockedNephalemCube, ToonSkillSet skillSet)
        {
            this.ToonHandle = new ToonHandleHelper(this.PersistentID);
            this.D3EntityID = this.ToonHandle.ToD3EntityID();
            this.BnetEntityID = this.ToonHandle.ToBnetEntityID();

            this.Name = name;
            this.HashCode = hashCode;
            this.HashCodeString = HashCode.ToString("D3");
            this.Class = @class;
            this.Flags = flags;
            this.Level = level;
            this.ExperienceNext = experience;
            this.Owner = owner;
            this.TimePlayed = timePlayed;

            this.CurrentQuest = -1;
            this.CurrentQuestStepId = -1;

            this.UnlockedStoneOfRecall = unlockedStoneOfRecall;
            this.UnlockedCauldronOfJordan = unlockedCauldronOfJordan;
            this.UnlockedNephalemCube = unlockedNephalemCube;

            this.SkillSet = skillSet;

            this.HeroTable = HeroData.Heros.Find(item => item.Name == this.Class.ToString());

            var visualItems = new[]
            {                                
                D3.Hero.VisualItem.CreateBuilder().SetGbid(0).SetDyeType(0).SetItemEffectType(0).SetEffectLevel(0).Build(), // Head
                D3.Hero.VisualItem.CreateBuilder().SetGbid(0).SetDyeType(0).SetItemEffectType(0).SetEffectLevel(0).Build(), // Chest
                D3.Hero.VisualItem.CreateBuilder().SetGbid(0).SetDyeType(0).SetItemEffectType(0).SetEffectLevel(0).Build(), // Feet
                D3.Hero.VisualItem.CreateBuilder().SetGbid(0).SetDyeType(0).SetItemEffectType(0).SetEffectLevel(0).Build(), // Hands
                D3.Hero.VisualItem.CreateBuilder().SetGbid(0).SetDyeType(0).SetItemEffectType(0).SetEffectLevel(0).Build(), // Weapon (1)
                D3.Hero.VisualItem.CreateBuilder().SetGbid(0).SetDyeType(0).SetItemEffectType(0).SetEffectLevel(0).Build(), // Weapon (2)
                D3.Hero.VisualItem.CreateBuilder().SetGbid(0).SetDyeType(0).SetItemEffectType(0).SetEffectLevel(0).Build(), // Shoulders
                D3.Hero.VisualItem.CreateBuilder().SetGbid(0).SetDyeType(0).SetItemEffectType(0).SetEffectLevel(0).Build(), // Legs
            };

            this.Equipment = D3.Hero.VisualEquipment.CreateBuilder().AddRangeVisualItem(visualItems).Build();

        }

        private void SetFields(string name, int hashCode, ToonClass @class, ToonFlags flags, byte level, Account owner, uint timePlayed)
        {
            ToonSkillSet skillSet = new ToonSkillSet(@class);
            this.SetFields(name, hashCode, @class, flags, level, Player.LevelBorders[level], owner, timePlayed, -1, -1, false, false, false, skillSet);
        }

        #endregion

        public void LevelUp()
        {
            this.Level++;
        }

        private static ToonClass GetClassByID(int classId)
        {
            switch (classId)
            {
                case 0x4FB91EE2:
                    return ToonClass.Barbarian;
                case unchecked((int)0xC88B9649):
                    return ToonClass.DemonHunter;
                case 0x3DAC15:
                    return ToonClass.Monk;
                case 0x343C22A:
                    return ToonClass.WitchDoctor;
                case 0x1D4681B1:
                    return ToonClass.Wizard;
            }

            return ToonClass.Barbarian;
        }

        public bnet.protocol.presence.Field QueryField(bnet.protocol.presence.FieldKey queryKey)
        {
            var field = bnet.protocol.presence.Field.CreateBuilder().SetKey(queryKey);

            switch ((FieldKeyHelper.Program)queryKey.Program)
            {
                case FieldKeyHelper.Program.D3:
                    if (queryKey.Group == 2 && queryKey.Field == 1) // Banner configuration
                    {
                        field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetMessageValue(this.Owner.BannerConfiguration.ToByteString()).Build());
                    }
                    else if (queryKey.Group == 3 && queryKey.Field == 1) // Hero's class (GbidClass)
                    {
                        field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetIntValue(this.ClassID).Build());
                    }
                    else if (queryKey.Group == 3 && queryKey.Field == 2) // Hero's current level
                    {
                        field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetIntValue(this.Level).Build());
                    }
                    else if (queryKey.Group == 3 && queryKey.Field == 3) // Hero's visible equipment
                    {
                        field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetMessageValue(this.Equipment.ToByteString()).Build());
                    }
                    else if (queryKey.Group == 3 && queryKey.Field == 4) // Hero's flags (gender and such)
                    {
                        field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetIntValue((uint)(this.Flags | ToonFlags.BothUnknowns)).Build());
                    }
                    else if (queryKey.Group == 4 && queryKey.Field == 1) // Channel ID if the client is online
                    {
                        if (this.Owner.LoggedInClient != null && this.Owner.LoggedInClient.CurrentChannel != null) field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetMessageValue(this.Owner.LoggedInClient.CurrentChannel.D3EntityId.ToByteString()).Build());
                        else field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().Build());
                    }
                    else if (queryKey.Group == 4 && queryKey.Field == 2) // Current screen (all known values are just "in-menu"; also see ScreenStatuses sent in ChannelService.UpdateChannelState)
                    {
                        field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetIntValue(0).Build());
                    }
                    else
                    {
                        Logger.Warn("Unknown query-key: {0}, {1}, {2}", queryKey.Program, queryKey.Group, queryKey.Field);
                    }
                    break;
                case FieldKeyHelper.Program.BNet:
                    if (queryKey.Group == 3 && queryKey.Field == 2) // Toon name
                    {
                        field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetStringValue(this.Name).Build());
                    }
                    else if (queryKey.Group == 3 && queryKey.Field == 3) // Whether the toon is selected one for owner account.
                    {
                        field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetBoolValue(this.IsSelected).Build());
                    }
                    else if (queryKey.Group == 3 && queryKey.Field == 5) // Away status - 0 for online
                    {
                        field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetIntValue((uint)this.AwayStatus).Build());
                    }
                    else if (queryKey.Group == 3 && queryKey.Field == 9) // Program - always D3
                    {
                        field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetFourccValue("D3").Build());
                    }
                    else
                    {
                        Logger.Warn("Unknown query-key: {0}, {1}, {2}", queryKey.Program, queryKey.Group, queryKey.Field);
                    }
                    break;
            }

            return field.HasValue ? field.Build() : null;
        }

        public void Update(bnet.protocol.presence.FieldOperation operation)
        {
            switch (operation.Operation)
            {
                case bnet.protocol.presence.FieldOperation.Types.OperationType.SET:
                    DoSet(operation.Field);
                    break;
                case bnet.protocol.presence.FieldOperation.Types.OperationType.CLEAR:
                    DoClear(operation.Field);
                    break;
            }
        }

        private void DoSet(bnet.protocol.presence.Field field)
        {
            switch ((FieldKeyHelper.Program)field.Key.Program)
            {
                case FieldKeyHelper.Program.D3:
                    if (field.Key.Group == 4 && field.Key.Field == 1)
                    {   
                        //don't know what to do with this yet, so far I have observed that the update value is always equal to the toon owner's current channel /dustinconrad
                        if (field.Value.HasMessageValue && !field.Value.MessageValue.Equals(this.Owner.LoggedInClient.CurrentChannel.D3EntityId.ToByteString()))
                        {
                            Logger.Warn("Toon owner's logged-in client channel is not equal to the channel specified in the update message");
                        }
                    }
                    else if (field.Key.Group == 4 && field.Key.Field == 2)
                    {
                        //catch to stop Logger.Warn spam on client start and exit
                        // should D3.4.2 int64 Current screen (0=in-menus, 1=in-menus, 3=in-menus); see ScreenStatus sent to ChannelService.UpdateChannelState call /raist
                    }
                    else if (field.Key.Group == 4 && field.Key.Field == 3)
                    {
                        //Looks to be the ToonFlags of the party leader/inviter when it is an int, OR the message set in an open to friends game when it is a string /dustinconrad
                    }
                    else
                    {
                        Logger.Warn("Unknown set-field: {0}, {1}, {2} := {3}", field.Key.Program, field.Key.Group, field.Key.Field, field.Value);
                    }
                    break;
                case FieldKeyHelper.Program.BNet:
                    if (field.Key.Group == 3 && field.Key.Field == 5) // Away status
                    {
                        AwayStatus = (AwayStatus)field.Value.IntValue;
                    }
                    else
                    {
                        Logger.Warn("Unknown set-field: {0}, {1}, {2} := {3}", field.Key.Program, field.Key.Group, field.Key.Field, field.Value);
                    }
                    break;
            }
        }

        private void DoClear(bnet.protocol.presence.Field field)
        {
            switch ((FieldKeyHelper.Program)field.Key.Program)
            {
                case FieldKeyHelper.Program.D3:
                    {
                        Logger.Warn("Unknown clear-field: {0}, {1}, {2}", field.Key.Program, field.Key.Group, field.Key.Field);
                    }
                    break;
                case FieldKeyHelper.Program.BNet:
                    {
                        Logger.Warn("Unknown clear-field: {0}, {1}, {2}", field.Key.Program, field.Key.Group, field.Key.Field);
                    }
                    break;
            }
        }

        protected override void NotifySubscriptionAdded(EmuNetClient client)
        {
            var operations = new List<bnet.protocol.presence.FieldOperation>();

            // Banner configuration
            var fieldKey1 = FieldKeyHelper.Create(FieldKeyHelper.Program.D3, 2, 1, 0);
            var field1 = bnet.protocol.presence.Field.CreateBuilder().SetKey(fieldKey1).SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetMessageValue(client.Account.BannerConfiguration.ToByteString()).Build()).Build();
            operations.Add(bnet.protocol.presence.FieldOperation.CreateBuilder().SetField(field1).Build());

            // Class
            var fieldKey2 = FieldKeyHelper.Create(FieldKeyHelper.Program.D3, 3, 1, 0);
            var field2 = bnet.protocol.presence.Field.CreateBuilder().SetKey(fieldKey2).SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetIntValue(this.ClassID).Build()).Build();
            operations.Add(bnet.protocol.presence.FieldOperation.CreateBuilder().SetField(field2).Build());

            // Level
            var fieldKey3 = FieldKeyHelper.Create(FieldKeyHelper.Program.D3, 3, 2, 0);
            var field3 = bnet.protocol.presence.Field.CreateBuilder().SetKey(fieldKey3).SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetIntValue(this.Level).Build()).Build();
            operations.Add(bnet.protocol.presence.FieldOperation.CreateBuilder().SetField(field3).Build());

            // Equipment
            var fieldKey4 = FieldKeyHelper.Create(FieldKeyHelper.Program.D3, 3, 3, 0);
            var field4 = bnet.protocol.presence.Field.CreateBuilder().SetKey(fieldKey4).SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetMessageValue(this.Equipment.ToByteString()).Build()).Build();
            operations.Add(bnet.protocol.presence.FieldOperation.CreateBuilder().SetField(field4).Build());

            // Flags
            var fieldKey5 = FieldKeyHelper.Create(FieldKeyHelper.Program.D3, 3, 4, 0);
            var field5 = bnet.protocol.presence.Field.CreateBuilder().SetKey(fieldKey5).SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetIntValue((uint)(this.Flags | ToonFlags.BothUnknowns)).Build()).Build();
            operations.Add(bnet.protocol.presence.FieldOperation.CreateBuilder().SetField(field5).Build());

            // Name
            var fieldKey6 = FieldKeyHelper.Create(FieldKeyHelper.Program.BNet, 3, 2, 0);
            var field6 = bnet.protocol.presence.Field.CreateBuilder().SetKey(fieldKey6).SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetStringValue(this.Name).Build()).Build();
            operations.Add(bnet.protocol.presence.FieldOperation.CreateBuilder().SetField(field6).Build());

            // Is it selected toon?
            var fieldKey7 = FieldKeyHelper.Create(FieldKeyHelper.Program.BNet, 3, 3, 0);
            var field7 = bnet.protocol.presence.Field.CreateBuilder().SetKey(fieldKey7).SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetBoolValue(this.IsSelected).Build()).Build();
            operations.Add(bnet.protocol.presence.FieldOperation.CreateBuilder().SetField(field7).Build());

            //AwayStatus - Available, Away, Busy
            var fieldKey10 = FieldKeyHelper.Create(FieldKeyHelper.Program.BNet, 3, 5, 0);
            var field10 = bnet.protocol.presence.Field.CreateBuilder().SetKey(fieldKey10).SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetIntValue((uint)(this.AwayStatus)).Build()).Build();
            operations.Add(bnet.protocol.presence.FieldOperation.CreateBuilder().SetField(field10).Build());

            // Program - FourCC "D3"
            var fieldKey8 = FieldKeyHelper.Create(FieldKeyHelper.Program.BNet, 3, 9, 0);
            var field8 = bnet.protocol.presence.Field.CreateBuilder().SetKey(fieldKey8).SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetFourccValue("D3").Build()).Build();
            operations.Add(bnet.protocol.presence.FieldOperation.CreateBuilder().SetField(field8).Build());

            // Unknown int - maybe highest completed act? /raist
            var fieldKey9 = FieldKeyHelper.Create(FieldKeyHelper.Program.BNet, 3, 9, 10);
            var field9 = bnet.protocol.presence.Field.CreateBuilder().SetKey(fieldKey9).SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetIntValue(0).Build()).Build();
            operations.Add(bnet.protocol.presence.FieldOperation.CreateBuilder().SetField(field9).Build());

            // Create a presence.ChannelState
            var state = bnet.protocol.presence.ChannelState.CreateBuilder().SetEntityId(this.BnetEntityID).AddRangeFieldOperation(operations).Build();

            // Embed in channel.ChannelState
            var channelState = bnet.protocol.channel.ChannelState.CreateBuilder().SetExtension(bnet.protocol.presence.ChannelState.Presence, state);

            // Put in AddNotification message
            var builder = bnet.protocol.channel.AddNotification.CreateBuilder().SetChannelState(channelState);

            // Make the RPC call
            client.MakeTargetedRPC(this, () => 
                bnet.protocol.channel.ChannelSubscriber.CreateStub(client).NotifyAdd(null, builder.Build(), callback => { }));
        }

        public override string ToString()
        {
            return String.Format("{{ Toon: {0} [lowId: {1}] }}", this.Name, this.BnetEntityID.Low);
        }

        public void SaveToDB()
        {
            try
            {
                if (ExistsInDB())
                {
                    var query =
                        string.Format(
                            "UPDATE toons SET name='{1}', hashCode={2}, class={3}, gender={4}, level={5}, experience={6}, accountId={7}, timePlayed={8}, currentQuest={9}, currentQuestStepId={10}, unlockedStoneOfRecall={11}, unlockedCauldronOfJordan={12}, unlockedNephalemCube={13} WHERE id={0}",
                            PersistentID, Name, HashCode, (byte)Class, (byte)Gender, Level, ExperienceNext, Owner.PersistentID, TimePlayed,
                            CurrentQuest, CurrentQuestStepId,
                            Convert.ToInt32(UnlockedStoneOfRecall), Convert.ToInt32(UnlockedCauldronOfJordan), Convert.ToInt32(UnlockedNephalemCube));

                    var cmd = new SQLiteCommand(query, DBManager.Connection);
                    cmd.ExecuteNonQuery();

                    query =
                        string.Format(
                            "UPDATE toon_skill_sets SET active0={1}, active1={2}, active2={3}, active3={4}, active4={5}, active5={6}, passive0={7}, passive1={8}, passive2={9}, hotbarButton0={10}, hotbarButton1={11}, hotbarButton2={12}, hotbarButton3={13}, hotbarButton4={14}, hotbarButton5={15}, hotbarButton6={16}, hotbarButton7={17}, hotbarButton8={18} WHERE toonId={0}",
                            PersistentID,
                            SkillSet.ActiveSkills[0], SkillSet.ActiveSkills[1], SkillSet.ActiveSkills[2], SkillSet.ActiveSkills[3], SkillSet.ActiveSkills[4], SkillSet.ActiveSkills[5],
                            SkillSet.PassiveSkills[0], SkillSet.PassiveSkills[1], SkillSet.PassiveSkills[2],
                            SkillSet.HotbarButtons[0], SkillSet.HotbarButtons[1], SkillSet.HotbarButtons[2], SkillSet.HotbarButtons[3], SkillSet.HotbarButtons[4], SkillSet.HotbarButtons[5], SkillSet.HotbarButtons[6], SkillSet.HotbarButtons[7], SkillSet.HotbarButtons[8]);
                    cmd = new SQLiteCommand(query, DBManager.Connection);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    var query =
                        string.Format(
                            "INSERT INTO toons (id, name, hashCode, class, gender, level, experience, accountId, timePlayed, currentQuest, currentQuestStepId, unlockedStoneOfRecall, unlockedCauldronOfJordan, unlockedNephalemCube) VALUES({0},'{1}',{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13})",
                            PersistentID, Name, HashCode, (byte)Class, (byte)Gender, Level, ExperienceNext, Owner.PersistentID, TimePlayed,
                            CurrentQuest, CurrentQuestStepId,
                            Convert.ToInt32(UnlockedStoneOfRecall), Convert.ToInt32(UnlockedCauldronOfJordan), Convert.ToInt32(UnlockedNephalemCube));

                    var cmd = new SQLiteCommand(query, DBManager.Connection);
                    cmd.ExecuteNonQuery();

                    query =
                        string.Format(
                            "INSERT INTO toon_skill_sets (toonId, active0, active1, active2, active3, active4, active5, passive0, passive1, passive2, hotbarButton0, hotbarButton1, hotbarButton2, hotbarButton3, hotbarButton4, hotbarButton5, hotbarButton6, hotbarButton7, hotbarButton8) VALUES({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18})",
                            PersistentID,
                            SkillSet.ActiveSkills[0], SkillSet.ActiveSkills[1], SkillSet.ActiveSkills[2], SkillSet.ActiveSkills[3], SkillSet.ActiveSkills[4], SkillSet.ActiveSkills[5],
                            SkillSet.PassiveSkills[0], SkillSet.PassiveSkills[1], SkillSet.PassiveSkills[2],
                            SkillSet.HotbarButtons[0], SkillSet.HotbarButtons[1], SkillSet.HotbarButtons[2], SkillSet.HotbarButtons[3], SkillSet.HotbarButtons[4], SkillSet.HotbarButtons[5], SkillSet.HotbarButtons[6], SkillSet.HotbarButtons[7], SkillSet.HotbarButtons[8]);
                    cmd = new SQLiteCommand(query, DBManager.Connection);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Logger.ErrorException(e, "Toon.SaveToDB()");
            }
        }

        public bool DeleteFromDB()
        {
            try
            {
                // Remove from DB
                if (!ExistsInDB()) return false;

                var query = string.Format("DELETE FROM toons WHERE id={0}", PersistentID);
                var cmd = new SQLiteCommand(query, DBManager.Connection);
                cmd.ExecuteNonQuery();

                query = string.Format("DELETE FROM toon_skill_sets WHERE toonId={0}", PersistentID);
                cmd = new SQLiteCommand(query, DBManager.Connection);
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception e)
            {
                Logger.ErrorException(e, "Toon.DeleteFromDB()");
                return false;
            }
        }

        private bool ExistsInDB()
        {
            var query =
                string.Format(
                    "SELECT id from toons where id={0}",
                    this.PersistentID);

            var cmd = new SQLiteCommand(query, DBManager.Connection);
            var reader = cmd.ExecuteReader();
            return reader.HasRows;
        }
    }

    public enum ToonClass
    {
        Barbarian, // 0x4FB91EE2
        Monk, // 0x3DAC15
        DemonHunter, // 0xC88B9649
        WitchDoctor, // 0x343C22A
        Wizard // 0x1D4681B1
    }

    [Flags]
    public enum ToonFlags : uint
    {
        Male = 0x00,
        Female = 0x02,
        // TODO: These two need to be figured out still.. /plash
        Unknown1 = 0x20,
        Unknown2 = 0x2000000,
        BothUnknowns = Unknown1 | Unknown2
    }

    //TODO: figure out what 1 and 3 represent, or if it is a flag since all observed values are powers of 2 so far /dustinconrad
    public enum AwayStatus : uint
    {
        Available = 0x00,
        UnknownStatus1 = 0x01,
        Away = 0x02,
        UnknownStatus2 = 0x03,
        Busy = 0x04
    }

}
