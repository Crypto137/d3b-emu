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
using D3BEmu.Common;
using D3BEmu.Common.Logging;
using D3BEmu.Net.EmuNet;

namespace D3BEmu.Core.EmuNet.Services
{
    [Service(serviceID: 0x11, serviceName: "bnet.protocol.channel.ChannelOwner")]
    public class ChannelOwnerService : bnet.protocol.channel.ChannelOwner, IServerService
    {
        private static readonly Logger Logger = LogManager.CreateLogger();
        public EmuNetClient Client { get; set; }

        public override void CreateChannel(Google.ProtocolBuffers.IRpcController controller, bnet.protocol.channel.CreateChannelRequest request, System.Action<bnet.protocol.channel.CreateChannelResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void FindChannel(Google.ProtocolBuffers.IRpcController controller, bnet.protocol.channel.FindChannelRequest request, System.Action<bnet.protocol.channel.FindChannelResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void GetChannelId(Google.ProtocolBuffers.IRpcController controller, bnet.protocol.channel.GetChannelIdRequest request, System.Action<bnet.protocol.channel.GetChannelIdResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void GetChannelInfo(Google.ProtocolBuffers.IRpcController controller, bnet.protocol.channel.GetChannelInfoRequest request, System.Action<bnet.protocol.channel.GetChannelInfoResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void JoinChannel(Google.ProtocolBuffers.IRpcController controller, bnet.protocol.channel.JoinChannelRequest request, System.Action<bnet.protocol.channel.JoinChannelResponse> done)
        {
            Logger.Warn("ChannelOwnerService:JoinChannel()");
            
            var builder = bnet.protocol.channel.JoinChannelResponse.CreateBuilder().SetObjectId(67122); // should be fixed with the actual joined channel object id.
            done(builder.Build());
        }
    }
}
