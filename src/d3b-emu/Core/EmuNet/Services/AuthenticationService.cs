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
using System.Globalization;
using System.Threading;
using D3BEmu.Common;
using D3BEmu.Common.Logging;
using D3BEmu.Core.EmuNet.Authentication;
using D3BEmu.Core.EmuNet.Online;
using D3BEmu.Net.EmuNet;

namespace D3BEmu.Core.EmuNet.Services
{
    [Service(serviceID: 0x1, serviceName: "bnet.protocol.authentication.AuthenticationServer")]
    public class AuthenticationService:bnet.protocol.authentication.AuthenticationServer, IServerService
    {
        private static readonly Logger Logger = LogManager.CreateLogger();
        public EmuNetClient Client { get; set; }
        
        public override void Logon(Google.ProtocolBuffers.IRpcController controller, bnet.protocol.authentication.LogonRequest request, Action<bnet.protocol.authentication.LogonResponse> done)
        {
            Logger.Trace("LogonRequest(); Email={0}", request.Email);

            // we should be also checking here version, program, locale and similar stuff /raist.

            AuthManager.StartAuthentication(this.Client, request);

            var authenticationThread = new Thread(() =>
            {
                this.Client.AuthenticationCompleteSignal.WaitOne(); // wait the signal;

                if(this.Client.AuthenticationErrorCode != EmuNetClient.AuthenticationErrorCodes.None)
                {
                    Logger.Info("Authentication failed for {0} because of invalid credentals.", request.Email);
                    done(bnet.protocol.authentication.LogonResponse.DefaultInstance);
                    return;
                }

                Logger.Info("User {0} authenticated successfuly.", request.Email);
                var builder = bnet.protocol.authentication.LogonResponse.
                    CreateBuilder()
                    .SetAccount(Client.Account.BnetAccountID)
                    .SetGameAccount(Client.Account.BnetGameAccountID);

                done(builder.Build());

                PlayerManager.PlayerConnected(this.Client);

            }) { IsBackground = true, CurrentCulture = CultureInfo.InvariantCulture }; ;

            authenticationThread.Start();
        }

        public override void ModuleMessage(Google.ProtocolBuffers.IRpcController controller, bnet.protocol.authentication.ModuleMessageRequest request, Action<bnet.protocol.NoData> done)
        {
            var moduleMessage = request.Message.ToByteArray();
            var command = moduleMessage[0];

            done(bnet.protocol.NoData.CreateBuilder().Build());

            if(request.ModuleId==0 && command==2)
                AuthManager.HandleAuthResponse(this.Client, request.ModuleId, moduleMessage);
        }
    }
}
