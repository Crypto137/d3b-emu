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

using D3BEmu.Common;
using D3BEmu.Common.Logging;

namespace D3BEmu.Net.GS
{
    public sealed class GameServer : Server
    {
        private new static readonly Logger Logger = LogManager.CreateLogger(); // hide the Server.Logger so that tiny-logger can show the actual server as log source.

        public GameServer()
        {
            this.OnConnect += ClientManager.Instance.OnConnect;
            this.OnDisconnect += ClientManager.Instance.OnDisconnect;
            this.DataReceived += GameServer_DataReceived;
            this.DataSent += (sender, e) => { };
        }

        void GameServer_DataReceived(object sender, ConnectionDataEventArgs e)
        {
            var connection = (Connection)e.Connection;
            ((GameClient)connection.Client).Parse(e);
        }

        public override void Run()
        {
            if (!this.Listen(Config.Instance.BindIP, Config.Instance.Port)) return;
            Logger.Info("Game-Server is listening on {0}:{1}...", Config.Instance.BindIP, Config.Instance.Port);
        }
    }
}
