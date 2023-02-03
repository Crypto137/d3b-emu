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

using System;
using System.Collections.Generic;
using System.Linq;
using D3BEmu.Common;
using D3BEmu.Common.Extensions;
using D3BEmu.Net.EmuNet;
using bnet.protocol;
using bnet.protocol.connection;
using D3BEmu.Common.Logging;

namespace D3BEmu.Core.EmuNet.Services
{
    [Service(serviceID: 0x0, serviceHash: 0x0)]
    public class BaseService :  ConnectionService,  IServerService
    {
        private static readonly Logger Logger = LogManager.CreateLogger();
        public EmuNetClient Client { get; set; }

        public override void Connect(Google.ProtocolBuffers.IRpcController controller, ConnectRequest request, Action<ConnectResponse> done)
        {
            Logger.Trace("Connect()");

            var builder = ConnectResponse.CreateBuilder()
                .SetServerId(ProcessId.CreateBuilder().SetLabel(0xAAAA).SetEpoch(DateTime.Now.ToUnixTime()))
                .SetClientId(ProcessId.CreateBuilder().SetLabel(0xBBBB).SetEpoch(DateTime.Now.ToUnixTime()));

            done(builder.Build());
        }

        public override void Bind(Google.ProtocolBuffers.IRpcController controller, BindRequest request, Action<BindResponse> done)
        {
            var requestedServiceIDs = new List<uint>();
            foreach (var serviceHash in request.ImportedServiceHashList)
            {
                var serviceID = Service.GetByHash(serviceHash);
                Logger.Trace("Bind() {0} [export] Hash: 0x{1} Id: 0x{2} Service: {3} ", this.Client, serviceHash.ToString("X8"), serviceID.ToString("X2"),

                Service.GetByID(serviceID) != null ? Service.GetByID(serviceID).GetType().Name : "N/A");
                requestedServiceIDs.Add(serviceID);
            }

            // read services supplied by client..
            foreach (var service in request.ExportedServiceList.Where(service => !Client.Services.ContainsValue(service.Id)))
            {
                if (Client.Services.ContainsKey(service.Hash)) continue;
                Client.Services.Add(service.Hash, service.Id);
                Logger.Trace(string.Format("Bind() {0} [import] Hash: 0x{1} Id: 0x{2}", this.Client, service.Hash.ToString("X8"), service.Id.ToString("X2")));
            }

            var builder = BindResponse.CreateBuilder();
            foreach (var serviceId in requestedServiceIDs) builder.AddImportedServiceId(serviceId);
            
            done(builder.Build());
        }

        public override void Echo(Google.ProtocolBuffers.IRpcController controller, EchoRequest request, Action<EchoResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void Encrypt(Google.ProtocolBuffers.IRpcController controller, EncryptRequest request, Action<NoData> done)
        {
            throw new NotImplementedException();
        }

        public override void ForceDisconnect(Google.ProtocolBuffers.IRpcController controller, DisconnectNotification request, Action<NO_RESPONSE> done)
        {
            throw new NotImplementedException();
        }

        public override void Null(Google.ProtocolBuffers.IRpcController controller, NullRequest request, Action<NO_RESPONSE> done)
        {
            throw new NotImplementedException();
        }

        public override void RequestDisconnect(Google.ProtocolBuffers.IRpcController controller, DisconnectRequest request, Action<NO_RESPONSE> done)
        {
            throw new NotImplementedException();
        }        
    }
}
