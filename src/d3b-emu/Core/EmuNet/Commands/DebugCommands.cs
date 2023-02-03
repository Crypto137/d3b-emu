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
using System.Linq;
using D3BEmu.Core.EmuNet.Accounts;
using D3BEmu.Core.EmuNet.Objects;
using D3BEmu.Core.EmuNet.Services;
using D3BEmu.Net.EmuNet;

namespace D3BEmu.Core.EmuNet.Commands
{
    [CommandGroup("services", "Lists emunet services provided by the server.\nUsage: services [client]", Account.UserLevels.Owner)]
    public class ServiceDebugCommands : CommandGroup
    {
        [DefaultCommand]
        public string Services(string[] @params, EmuNetClient invokerClient)
        {
            return Service.ProvidedServices.Aggregate("Provided services by server:\n",
                (current, pair) => current + string.Format("Id: 0x{0} Hash: 0x{1} [{2}]\n", pair.Value.ServiceID.ToString("X2"), pair.Value.Hash.ToString("X8"), pair.Key.Name));
        }

        [Command("client", "Shows imported service list for client.\nUsage: services client [email]")]
        public string ClientServices(string[] @params, EmuNetClient invokerClient)
        {
            var client = invokerClient;

            if(client==null && @params.Count() < 1)
                return "Invalid arguments. Type 'help services client' to get help.";

            if (client == null)
            {
                var email = @params[0];
                var account = AccountManager.GetAccountByEmail(email);

                if (account == null)
                    return string.Format("No account with email '{0}' exists.", email);

                client = account.LoggedInClient;
                if (client == null)
                    return string.Format("Account '{0}' is not logged in.", email);
            }

            var output = string.Format("Imported service list for client: {0}\n", client.Account.Email);
            output = client.Services.Aggregate(output, (current, pair) => 
                current + string.Format("Id: 0x{0} Hash: 0x{1}\n", pair.Value.ToString("X2"), pair.Key.ToString("X8")));

            return output;
        }
    }

    [CommandGroup("rpcobject", "Lists rpc-objects.", Account.UserLevels.Owner)]
    public class RPCObjectDebugCommands : CommandGroup
    {
        [Command("list", "Shows lists of RPCObjects")]
        public string List(string[] @params, EmuNetClient invokerClient)
        {
            return RPCObjectManager.Objects.Aggregate("RPCObjects:\n",
                (current, pair) => current + string.Format("Id: 0x{0} - {1} [{2}]\n", pair.Key.ToString("X8"), pair.Value.GetType().Name, pair.Value.ToString()));
        }

        [Command("show", "Prints detailed debug information for RPCObject.\nUsage: rpcobject show <localId>")]
        public string Show(string[] @params, EmuNetClient invokerClient)
        {
            if (@params.Count() < 1)
                return "Invalid arguments. Type 'help rpcobject show' to get help.";

            ulong localId;
            var id = @params[0];
            if (!ulong.TryParse(id, out localId))
                return string.Format("Can not parse '{0}' as valid id.", id);

            if(!RPCObjectManager.Objects.ContainsKey(localId))
                return string.Format("There exists no RPCObject with dynamidId: {0}", localId);

            var rpcObject = RPCObjectManager.Objects[localId];
            var output = string.Format("[RPCObject]\nDynamicId: 0x{0}\nType: {1}\nObject: {2}\n", rpcObject.DynamicId,
                                       rpcObject.GetType().Name, rpcObject);

            output += "[Subscribers]\n";
            foreach(var client in rpcObject.Subscribers)
            {
                var remoteId = client.GetRemoteObjectID(rpcObject.DynamicId);
                output += string.Format("RemoteId: 0x{0} - {1}\n", remoteId.ToString("X8"), client.Account.Email);
            }

            return output;
        }
    }
}
