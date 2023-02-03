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
using System.Linq;
using Google.ProtocolBuffers;
using D3BEmu.Common.Logging;
using D3BEmu.Common.Extensions;
using D3BEmu.Core.Cryptography;
using D3BEmu.Core.EmuNet.Accounts;
using D3BEmu.Net.EmuNet;

namespace D3BEmu.Core.EmuNet.Authentication
{
    public static class AuthManager
    {
        private static readonly Logger Logger = LogManager.CreateLogger();
        private static readonly Dictionary<EmuNetClient, SRP6a> OngoingAuthentications = new Dictionary<EmuNetClient, SRP6a>();
        private static readonly byte[] ModuleHash = "8F52906A2C85B416A595702251570F96D3522F39237603115F2F1AB24962043C".ToByteArray(); // Password.dll

        public static void StartAuthentication(EmuNetClient client, bnet.protocol.authentication.LogonRequest request)
        {
            if (Config.Instance.BypassAuthentication)
                BypassAuthentication(client, request);
            else
                InitAuthentication(client, request);                                               
        }

        private static void BypassAuthentication(EmuNetClient client, bnet.protocol.authentication.LogonRequest request)
        {
            client.Account = AccountManager.GetAccountByEmail(request.Email) ?? AccountManager.CreateAccount(request.Email, request.Email);
            if (client.Account.LoggedInClient != null)
                client.Account.LoggedInClient.Connection.Disconnect();
            client.Account.LoggedInClient = client;

            client.AuthenticationCompleteSignal.Set(); // signal about completion of authentication processes so we can return the response for AuthenticationService:LogonRequest.
        }

        private static void InitAuthentication(EmuNetClient client, bnet.protocol.authentication.LogonRequest request)
        {
            var account = AccountManager.GetAccountByEmail(request.Email); // check if account exists.
            
            if (account == null) // we should be returning an error to client /raist.
            {
                client.AuthenticationErrorCode = EmuNetClient.AuthenticationErrorCodes.NoGameAccount;
                client.AuthenticationCompleteSignal.Set();
                return;
            }

            var srp6a = new SRP6a(account); // create srp6 handler to process the authentication.
            OngoingAuthentications.Add(client, srp6a);

            // request client to load password.dll for authentication.
            var moduleLoadRequest = bnet.protocol.authentication.ModuleLoadRequest.CreateBuilder()
                .SetModuleHandle(bnet.protocol.ContentHandle.CreateBuilder()
                    .SetRegion(0x00005553) // us
                    .SetUsage(0x61757468) // auth - password.dll
                    .SetHash(ByteString.CopyFrom(ModuleHash)))
                .SetMessage(ByteString.CopyFrom(srp6a.LogonChallenge))
                .Build();

            client.MakeRPCWithListenerId(request.ListenerId, () =>
                bnet.protocol.authentication.AuthenticationClient.CreateStub(client).ModuleLoad(null, moduleLoadRequest, ModuleLoadResponse));
        }

        public static void HandleAuthResponse(EmuNetClient client, int moduleId, byte[] authMessage)
        {
            if(!OngoingAuthentications.ContainsKey(client)) return; // TODO: disconnect him also. /raist.

            var srp6 = OngoingAuthentications[client];
            byte[] A = authMessage.Skip(1).Take(128).ToArray(); // client's public ephemeral
            byte[] M_client = authMessage.Skip(1 + 128).Take(32).ToArray(); // client's proof of session key.
            byte[] seed = authMessage.Skip(1 + 32 + 128).Take(128).ToArray(); // client's second challenge.

            if(srp6.Verify(A,M_client,seed)) // authentication sucesseful
            {
                // send the logon proof.
                var message = bnet.protocol.authentication.ModuleMessageRequest.CreateBuilder()
                    .SetModuleId(moduleId)
                    .SetMessage(ByteString.CopyFrom(srp6.LogonProof))
                    .Build();

                client.MakeRPC(() =>
                    bnet.protocol.authentication.AuthenticationClient.CreateStub(client).ModuleMessage(null, message, callback => { }));

                client.Account = AccountManager.GetAccountByEmail(srp6.Account.Email);
                if (client.Account.LoggedInClient != null)
                    client.Account.LoggedInClient.Connection.Disconnect();
                client.Account.LoggedInClient = client;
            }
            else // authentication failed because of invalid credentals.
            {
                client.AuthenticationErrorCode = EmuNetClient.AuthenticationErrorCodes.InvalidCredentials;
            }
             
            OngoingAuthentications.Remove(client);
            client.AuthenticationCompleteSignal.Set(); // signal about completion of authentication processes so we can return the response for AuthenticationService:LogonRequest.
        }

        private static void ModuleLoadResponse(IMessage response)
        {
            Logger.Trace("{0}", response.ToString());
        }
    }
}
