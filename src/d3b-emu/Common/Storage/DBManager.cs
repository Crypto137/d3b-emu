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
using System.Data.SQLite;
using D3BEmu.Common.Helpers;
using D3BEmu.Common.Helpers.IO;
using D3BEmu.Common.Logging;

namespace D3BEmu.Common.Storage
{
    // just a quick hack - not to be meant a final layer.
    public static class DBManager
    {
        public static SQLiteConnection Connection { get; private set; }
        public static readonly Logger Logger = LogManager.CreateLogger();

        static DBManager()
        {
            Connect();            
        }

        private static void Connect()
        {
            try
            {
                Connection = new SQLiteConnection(String.Format("Data Source={0}/{1}/account.db", FileHelpers.AssemblyRoot, Config.Instance.Root));
                Connection.Open();
            }
            catch (Exception e)
            {
                Logger.FatalException(e, "Connect()");
            }
        }
    }
}
