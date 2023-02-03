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
using Google.ProtocolBuffers;
using D3BEmu.Common;
using D3BEmu.Net.EmuNet;
using bnet.protocol;
using bnet.protocol.search;
using D3BEmu.Common.Logging;

namespace D3BEmu.Core.EmuNet.Services
{
    [Service(serviceID: 0xe, serviceName: "bnet.protocol.search.SearchService")]
    public class SearchService : bnet.protocol.search.SearchService,IServerService
    {
        private static readonly Logger Logger = LogManager.CreateLogger();
        public EmuNetClient Client { get; set; }

        public override void FindMatches(IRpcController controller, FindMatchesRequest request, Action<FindMatchesResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void SetObject(IRpcController controller, SetObjectRequest request, Action<NO_RESPONSE> done)
        {
            throw new NotImplementedException();
        }

        public override void RemoveObjects(IRpcController controller, RemoveObjectsRequest request, Action<NO_RESPONSE> done)
        {
            throw new NotImplementedException();
        }
    }
}
