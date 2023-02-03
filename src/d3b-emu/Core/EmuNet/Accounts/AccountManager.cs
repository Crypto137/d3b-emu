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
using D3BEmu.Common;
using D3BEmu.Common.Logging;
using D3BEmu.Common.Storage;
using D3BEmu.Core.EmuNet.Commands;
using D3BEmu.Core.EmuNet.Online;

namespace D3BEmu.Core.EmuNet.Accounts
{
    public static class AccountManager
    {
        private static readonly Logger Logger = LogManager.CreateLogger();
        private static readonly Dictionary<string, Account> Accounts = new Dictionary<string, Account>();

        public static int TotalAccounts
        {
            get { return Accounts.Count; }
        }

        static AccountManager()
        {
            Accounts.Add(CommandHandlerAccount.Instance.Email, CommandHandlerAccount.Instance); // Hackish command handler account that we can send server commands. /raist
            CommandHandlerAccount.Instance.LoggedInClient.CurrentToon = CommandHandlerToon.Instance;
            PlayerManager.OnlinePlayers.Add(CommandHandlerAccount.Instance.LoggedInClient);

            LoadAccounts();
        }

        public static Account GetAccountByEmail(string email)
        {
            return Accounts.ContainsKey(email) ? Accounts[email] : null;
        }

        public static Account CreateAccount(string email, string password, Account.UserLevels userLevel = Account.UserLevels.User)
        {
            var account = new Account(email, password, userLevel);
            Accounts.Add(email, account);
            account.SaveToDB();

            return account;
        }

        public static Account GetAccountByPersistentID(ulong persistentId)
        {
            return Accounts.Where(account => account.Value.PersistentID == persistentId).Select(account => account.Value).FirstOrDefault();
        }

        public static bool DeleteAccount(Account account)
        {
            if (account == null) return false;
            if (!Accounts.ContainsKey(account.Email)) return false;

            try
            {
                var query = string.Format("DELETE from accounts where id={0}", account.PersistentID);
                var cmd = new SQLiteCommand(query, DBManager.Connection);
                cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                Logger.ErrorException(e, "DeleteAccount()");
                return false;
            }

            Accounts.Remove(account.Email);
            // we should be also disconnecting the account if he's online. /raist.

            return true;
        }

        private static void LoadAccounts()
        {
            var query = "SELECT * from accounts";
            var cmd = new SQLiteCommand(query, DBManager.Connection);
            var reader = cmd.ExecuteReader();

            if (!reader.HasRows) return;

            while (reader.Read())
            {
                ulong accountId = (ulong)reader.GetInt64(0);
                string email = reader.GetString(1);

                byte[] salt = new byte[32];
                reader.GetBytes(2, 0, salt, 0, 32);

                byte[] passwordVerifier = new byte[128];
                reader.GetBytes(3, 0, passwordVerifier, 0, 128);

                byte userLevel = reader.GetByte(4);

                byte[] bannerConfiguration = new byte[18];
                reader.GetBytes(5, 0, bannerConfiguration, 0, 18);

                int gold = reader.GetInt32(6);
                int stashSize = reader.GetInt32(7);

                ulong lastPlayedHeroId = reader.IsDBNull(8) ? 0 : (ulong)reader.GetInt64(8);

                Account account = new Account(accountId, email, salt, passwordVerifier, (Account.UserLevels)userLevel, bannerConfiguration, gold, stashSize, lastPlayedHeroId);
                Accounts.Add(email, account);
            }
        }

        public static ulong GetNextAvailablePersistentId()
        {
            var cmd = new SQLiteCommand("SELECT max(id) from accounts", DBManager.Connection);
            try
            {
                return Convert.ToUInt64(cmd.ExecuteScalar());
            }
            catch (InvalidCastException)
            {
                return 0;
            }
        }       
    }
}
