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

using System.Net;
using D3BEmu.Net.EmuNet;

namespace D3BEmu.Net
{
    public static class Utils
    {
        public static readonly IPAddress LocalHost = IPAddress.Parse("127.0.0.1");

        public static string GetGameServerIPForClient(EmuNetClient client)
        {
            if (!NATConfig.Instance.Enabled) // if NAT is not enabled, just return bnetclient's localendpoint address.
                return client.Connection.LocalEndPoint.Address.ToString();
            else
            {
                return client.Connection.LocalEndPoint.Address.ToString() == "127.0.0.1"
                           ? client.Connection.LocalEndPoint.ToString()
                           : NATConfig.Instance.PublicIP; // if client is not connected over localhost, send him public-ip.

                // Known problems: If user enables NAT, LAN-clients (and even local-computer if d3 is configured to use lan-ip) will not able to connect in gs.
                // That needs a full implementation similar to pvpgn where we currently pretty miss the time for /raist.
            }
        }       
    }
}
