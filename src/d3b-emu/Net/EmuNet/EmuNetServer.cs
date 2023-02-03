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
using D3BEmu.Core.EmuNet.Online;

namespace D3BEmu.Net.EmuNet
{
    public sealed class EmuNetServer : Server
    {
        private new static readonly Logger Logger = LogManager.CreateLogger(); // hide the Server.Logger so that tiny-logger can show the actual server as log source.

        public EmuNetServer()
        {
            this.OnConnect += EmuNetServer_OnConnect;
            this.OnDisconnect += EmuNetServer_OnDisconnect;
            this.DataReceived += (sender, e) => EmuNetRouter.Route(e);
            this.DataSent += (sender, e) => { };
        }

        private void EmuNetServer_OnConnect(object sender, ConnectionEventArgs e)
        {
            Logger.Trace("EmuNet-Client connected: {0}", e.Connection.ToString());
            e.Connection.Client = new EmuNetClient(e.Connection);
        }

        private void EmuNetServer_OnDisconnect(object sender, ConnectionEventArgs e)
        {
            var client = ((EmuNetClient) e.Connection.Client);

            Logger.Trace("Client disconnected: {0}", e.Connection.ToString());
            if (client.Account != null) client.Account.LoggedInClient = null;
            PlayerManager.PlayerDisconnected((EmuNetClient)e.Connection.Client);
        }
        
        public override void Run()
        {
            // we can't listen for port 1119 because D3 and the launcher (agent) communicates on that port through loopback.
            // so we change our default port and start D3 with a shortcut like so:
            //   "F:\Diablo III Beta\Diablo III.exe" -launch -auroraaddress 127.0.0.1:1345

            if (!this.Listen(Config.Instance.BindIP, Config.Instance.Port)) return;
            Logger.Info("EmuNet-Server is listening on {0}:{1}...", Config.Instance.BindIP, Config.Instance.Port);
        }
    }
}
