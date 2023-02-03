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

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using D3BEmu.Common.Storage;
using D3BEmu.Core.Cryptography;
using D3BEmu.Core.EmuNet.Friends;
using D3BEmu.Core.EmuNet.Helpers;
using D3BEmu.Core.EmuNet.Objects;
using D3BEmu.Core.EmuNet.Toons;
using D3BEmu.Net.EmuNet;

namespace D3BEmu.Core.EmuNet.Accounts
{
    public class Account : PersistentRPCObject
    {
        public bnet.protocol.EntityId BnetAccountID { get; private set; }
        public bnet.protocol.EntityId BnetGameAccountID { get; private set; }
        
        public string Email { get; private set; } // I - Username
        public byte[] Salt { get; private set; }  // s- User's salt.
        public byte[] PasswordVerifier { get; private set; } // v - password verifier.
        public UserLevels UserLevel { get; private set; } // user level for account.

        public bool IsOnline { get { return this.LoggedInClient != null; } }

        public D3.Account.BannerConfiguration BannerConfiguration { get; private set; }
        public int Gold { get; private set; }
        public int StashSize { get; private set; }

        private ulong _lastPlayedHeroIdLow;
        public D3.OnlineService.EntityId LastPlayedHeroId
        {
            get
            {
                if (this.LoggedInClient.CurrentToon == null)
                {
                    if (_lastPlayedHeroIdLow == 0)
                    {
                        return Toons.Count > 0 ? Toons.First().Value.D3EntityID : AccountHasNoToons;
                    }
                    else
                    {
                        return ToonManager.GetToonByLowID(_lastPlayedHeroIdLow).D3EntityID;
                    }
                }
                return this.LoggedInClient.CurrentToon.D3EntityID;
            }
            set
            {
                this.LoggedInClient.CurrentToon = ToonManager.GetToonByLowID(value.IdLow);
            }
        }

        private static readonly D3.OnlineService.EntityId AccountHasNoToons =
            D3.OnlineService.EntityId.CreateBuilder().SetIdHigh(0).SetIdLow(0).Build();

        public D3.Account.Digest Digest
        {
            get
            {
                var builder = D3.Account.Digest.CreateBuilder().SetVersion(99)
                    .SetBannerConfiguration(this.BannerConfiguration)
                    .SetFlags(0)
                    .SetLastPlayedHeroId(this.LastPlayedHeroId);

                return builder.Build();
            }
        }

        private EmuNetClient _loggedInClient;

        public EmuNetClient LoggedInClient
        {
            get
            {
                return this._loggedInClient;
            }
            set
            {
                this._loggedInClient = value;
                
                // notify friends.
                if (FriendManager.Friends[this.BnetAccountID.Low].Count == 0) return; // if account has no friends just skip.

                var fieldKey = FieldKeyHelper.Create(FieldKeyHelper.Program.BNet, 1, 2, 0);
                var field = bnet.protocol.presence.Field.CreateBuilder().SetKey(fieldKey).SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetBoolValue(this.IsOnline).Build()).Build();
                var operation = bnet.protocol.presence.FieldOperation.CreateBuilder().SetField(field).Build();

                var state = bnet.protocol.presence.ChannelState.CreateBuilder().SetEntityId(this.BnetAccountID).AddFieldOperation(operation).Build();
                var channelState = bnet.protocol.channel.ChannelState.CreateBuilder().SetExtension(bnet.protocol.presence.ChannelState.Presence, state);
                var notification = bnet.protocol.channel.UpdateChannelStateNotification.CreateBuilder().SetStateChange(channelState).Build();

                foreach (var friend in FriendManager.Friends[this.BnetAccountID.Low])
                {
                    var account = AccountManager.GetAccountByPersistentID(friend.Id.Low);
                    if (account == null || account.LoggedInClient == null) return; // only send to friends that are online.

                    // make the rpc call.
                    account.LoggedInClient.MakeTargetedRPC(this, ()=> 
                        bnet.protocol.channel.ChannelSubscriber.CreateStub(account.LoggedInClient).NotifyUpdateChannelState(null, notification,callback => { }));
                }
            }
        }

        public Dictionary<ulong, Toon> Toons
        {
            get { return ToonManager.GetToonsForAccount(this); }
        }

        public Account(ulong persistentId, string email, byte[] salt, byte[] passwordVerifier, UserLevels userLevel, byte[] bannerConfiguration, int gold, int stashSize, ulong lastPlayedHeroId) // Account with given persistent ID
            : base(persistentId)
        {
            this.SetFields(email, salt, passwordVerifier, userLevel, bannerConfiguration, gold, stashSize, lastPlayedHeroId);
        }

        public Account(string email, string password, UserLevels userLevel) // Account with **newly generated** persistent ID
            : base()
        {
            if (password.Length > 16) password = password.Substring(0, 16); // make sure the password does not exceed 16 chars.

            byte[] salt = SRP6a.GetRandomBytes(32);
            byte[] passwordVerifier = SRP6a.CalculatePasswordVerifierForAccount(email, password, salt);
            byte[] bannerConfiguration = D3.Account.BannerConfiguration.CreateBuilder()
                .SetBackgroundColorIndex(0)
                .SetBannerIndex(0)
                .SetPattern(0)
                .SetPatternColorIndex(0)
                .SetPlacementIndex(0)
                .SetSigilAccent(0)
                .SetSigilMain(0)
                .SetSigilColorIndex(0)
                .SetUseSigilVariant(false)
                .Build().ToByteArray();
            int gold = 0;
            int stashSize = 14;
            ulong lastPlayedHeroId = 0;

            this.SetFields(email, salt, passwordVerifier, userLevel, bannerConfiguration, gold, stashSize, lastPlayedHeroId);
        }

        private static ulong? _persistentIdCounter = null;
        protected override ulong GenerateNewPersistentId()
        {
            if (_persistentIdCounter == null)
                _persistentIdCounter = AccountManager.GetNextAvailablePersistentId();

            return (ulong)++_persistentIdCounter;
        }

        private void SetFields(string email, byte[] salt, byte[] passwordVerifier, UserLevels userLevel, byte[] bannerConfiguration, int gold, int stashSize, ulong lastPlayedHeroId)
        {
            this.BnetAccountID = bnet.protocol.EntityId.CreateBuilder().SetHigh((ulong)EntityIdHelper.HighIdType.AccountId).SetLow(this.PersistentID).Build();
            this.BnetGameAccountID = bnet.protocol.EntityId.CreateBuilder().SetHigh((ulong)EntityIdHelper.HighIdType.GameAccountId).SetLow(this.PersistentID).Build();

            this.Email = email;
            this.Salt = salt;
            this.PasswordVerifier = passwordVerifier;
            this.UserLevel = userLevel;

            this.BannerConfiguration = D3.Account.BannerConfiguration.ParseFrom(bannerConfiguration);
            this.Gold = gold;
            this.StashSize = stashSize;

            this._lastPlayedHeroIdLow = lastPlayedHeroId;
        }

        public bnet.protocol.presence.Field QueryField(bnet.protocol.presence.FieldKey queryKey)
        {
            var field = bnet.protocol.presence.Field.CreateBuilder().SetKey(queryKey);

            switch ((FieldKeyHelper.Program)queryKey.Program)
            {
                case FieldKeyHelper.Program.D3:
                    if (queryKey.Group == 1 && queryKey.Field == 1) // Account's selected toon.
                    {
                        if(this.LoggedInClient!=null) // check if the account is online actually.
                        field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetMessageValue(this.LoggedInClient.CurrentToon.D3EntityID.ToByteString()).Build());
                    }
                    else
                    {
                        Logger.Warn("Unknown query-key: {0}, {1}, {2}", queryKey.Program, queryKey.Group, queryKey.Field);
                    }
                    break;
                case FieldKeyHelper.Program.BNet:
                    Logger.Warn("Unknown query-key: {0}, {1}, {2}", queryKey.Program, queryKey.Group, queryKey.Field);
                    break;
            }


            return field.HasValue ? field.Build() : null;
        }

        protected override void NotifySubscriptionAdded(EmuNetClient client)
        {
            var operations = new List<bnet.protocol.presence.FieldOperation>();

            // RealID name field - NOTE: Probably won't ever use this for its actual purpose, but showing the email in final might not be a good idea
            var fieldKey1 = FieldKeyHelper.Create(FieldKeyHelper.Program.BNet,1, 1, 0);
            var field1 = bnet.protocol.presence.Field.CreateBuilder().SetKey(fieldKey1).SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetStringValue(this.Email).Build()).Build();
            operations.Add(bnet.protocol.presence.FieldOperation.CreateBuilder().SetField(field1).Build());

            // Account online?
            var fieldKey2 = FieldKeyHelper.Create(FieldKeyHelper.Program.BNet, 1, 2, 0);
            var field2 = bnet.protocol.presence.Field.CreateBuilder().SetKey(fieldKey2).SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetBoolValue(this.IsOnline).Build()).Build();
            operations.Add(bnet.protocol.presence.FieldOperation.CreateBuilder().SetField(field2).Build());

            // Selected toon
            if (this.LoggedInClient != null && this.LoggedInClient.CurrentToon!=null)
            {
                var fieldKey3 = FieldKeyHelper.Create(FieldKeyHelper.Program.D3, 1, 1, 0);
                var field3 = bnet.protocol.presence.Field.CreateBuilder().SetKey(fieldKey3).SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetMessageValue(this.LoggedInClient.CurrentToon.D3EntityID.ToByteString()).Build()).Build();
                operations.Add(bnet.protocol.presence.FieldOperation.CreateBuilder().SetField(field3).Build());
            }

            // toon list
            foreach(var pair in this.Toons)
            {
                var fieldKey = FieldKeyHelper.Create(FieldKeyHelper.Program.BNet, 1, 4, 0);
                var field = bnet.protocol.presence.Field.CreateBuilder().SetKey(fieldKey).SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetMessageValue(pair.Value.BnetEntityID.ToByteString()).Build()).Build();
                operations.Add(bnet.protocol.presence.FieldOperation.CreateBuilder().SetField(field).Build());
            }

            // Create a presence.ChannelState
            var state = bnet.protocol.presence.ChannelState.CreateBuilder().SetEntityId(this.BnetAccountID).AddRangeFieldOperation(operations).Build();

            // Embed in channel.ChannelState
            var channelState = bnet.protocol.channel.ChannelState.CreateBuilder().SetExtension(bnet.protocol.presence.ChannelState.Presence, state);

            // Put in addnotification message
            var notification = bnet.protocol.channel.AddNotification.CreateBuilder().SetChannelState(channelState);

            // Make the rpc call
            client.MakeTargetedRPC(this, () =>
                bnet.protocol.channel.ChannelSubscriber.CreateStub(client).NotifyAdd(null, notification.Build(), callback => { }));
        }

        public void SaveToDB()
        {
            try
            {
                var query = string.Format("INSERT INTO accounts (id, email, salt, passwordVerifier, userLevel, banner, gold, stashSize) VALUES({0}, '{1}', @salt, @passwordVerifier, {2}, @banner, {3}, {4})",
                        this.PersistentID, this.Email, (byte)this.UserLevel, this.Gold, this.StashSize);

                    using(var cmd = new SQLiteCommand(query, DBManager.Connection))
                    {
                        cmd.Parameters.Add("@salt", System.Data.DbType.Binary, 32).Value = this.Salt;
                        cmd.Parameters.Add("@passwordVerifier", System.Data.DbType.Binary, 128).Value = this.PasswordVerifier;
                        cmd.Parameters.Add("@banner", System.Data.DbType.Binary, 18).Value = this.BannerConfiguration.ToByteArray();
                        cmd.ExecuteNonQuery();
                    }                    
            }
            catch (Exception e)
            {
                Logger.ErrorException(e, "SaveToDB()");
            }
        }

        public void UpdatePassword(string newPassword)
        {
            this.PasswordVerifier = SRP6a.CalculatePasswordVerifierForAccount(this.Email, newPassword, this.Salt);
            try
            {
                var query = string.Format("UPDATE accounts SET passwordVerifier=@passwordVerifier WHERE id={0}", this.PersistentID);
                
                using (var cmd = new SQLiteCommand(query, DBManager.Connection))
                {
                    cmd.Parameters.Add("@passwordVerifier", System.Data.DbType.Binary, 128).Value = this.PasswordVerifier;
                    cmd.ExecuteNonQuery();
                }    
            }
            catch (Exception e)
            {
                Logger.ErrorException(e, "UpdatePassword()");
            }
        }

        public void UpdateUserLevel(UserLevels userLevel)
        {
            this.UserLevel = userLevel;
            try
            {
                var query = string.Format("UPDATE accounts SET userLevel={0} WHERE id={1}", (byte)userLevel, this.PersistentID);
                var cmd = new SQLiteCommand(query, DBManager.Connection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.ErrorException(e, "UpdateUserLevel()");
            }
        }

        public void UpdateBanner(D3.Account.BannerConfiguration bannerConfiguration)
        {
            this.BannerConfiguration = bannerConfiguration;
            try
            {
                var query = string.Format("UPDATE accounts SET banner=@banner WHERE id={0}", this.PersistentID);

                using (var cmd = new SQLiteCommand(query, DBManager.Connection))
                {
                    cmd.Parameters.Add("@banner", System.Data.DbType.Binary, 128).Value = this.BannerConfiguration.ToByteArray(); ;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Logger.ErrorException(e, "UpdateBanner()");
            }
        }

        public void UpdateGameData(int gold, int stashSize)
        {
            this.Gold = gold;
            this.StashSize = stashSize;
            try
            {
                var query = string.Format("UPDATE accounts SET gold={0}, stashSize={1} WHERE id={2}", gold, stashSize, this.PersistentID);
                var cmd = new SQLiteCommand(query, DBManager.Connection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.ErrorException(e, "UpdateGameData()");
            }
        }

        public void UpdateLastPlayedHeroId(ulong heroId)
        {
            this._lastPlayedHeroIdLow = heroId;
            try
            {
                var query = string.Format("UPDATE accounts SET lastPlayedHeroId={0} WHERE id={1}", heroId, this.PersistentID);
                var cmd = new SQLiteCommand(query, DBManager.Connection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.ErrorException(e, "UpdateLastPlayedHeroId()");
            }
        }

        public override string ToString()
        {
            return String.Format("{{ Account: {0} [lowId: {1}] }}", this.Email, this.BnetAccountID.Low);
        }

        /// <summary>
        /// User-levels.
        /// </summary>
        public enum UserLevels : byte
        {
            User,
            GM,
            Admin,
            Owner
        }
    }
}
