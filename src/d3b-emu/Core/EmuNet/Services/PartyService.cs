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
using D3BEmu.Common.Logging;
using D3BEmu.Core.EmuNet.Channels;
using D3BEmu.Net.EmuNet;

namespace D3BEmu.Core.EmuNet.Services
{
    [Service(serviceID: 0x0D, serviceName: "bnet.protocol.party.PartyService")]
    public class PartyService : bnet.protocol.party.PartyService,IServerService
    {
        private static readonly Logger Logger = LogManager.CreateLogger();
        public EmuNetClient Client { get; set; }

        // PartyService just uses ChannelService to create a new channel for the party.
        public override void CreateChannel(IRpcController controller, bnet.protocol.channel.CreateChannelRequest request, Action<bnet.protocol.channel.CreateChannelResponse> done)
        {
            var channel = ChannelManager.CreateNewChannel(this.Client, request.ObjectId);
            var builder = bnet.protocol.channel.CreateChannelResponse.CreateBuilder()
                .SetObjectId(channel.DynamicId)
                .SetChannelId(channel.BnetEntityId);

            done(builder.Build());
            channel.SetOwner(Client); // Set the client that requested the creation of channel as the owner

            Logger.Trace("CreateChannel() {0} for {1}", channel, Client.CurrentToon);

            // send our MOTD - though this is actually not the right place for it /raist.
            if (!this.Client.MOTDSent)
                this.Client.SendMOTD(); // send the MOTD to client if we haven't yet so.
        }

        public override void JoinChannel(IRpcController controller, bnet.protocol.channel.JoinChannelRequest request, Action<bnet.protocol.channel.JoinChannelResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void GetChannelInfo(IRpcController controller, bnet.protocol.channel.GetChannelInfoRequest request, Action<bnet.protocol.channel.GetChannelInfoResponse> done)
        {
            Logger.Trace("GetChannelInfoRequest() to channel {0}:{1} by toon {2}", request.ChannelId.High, request.ChannelId.Low, Client.CurrentToon.Name);

            var builder = bnet.protocol.channel.GetChannelInfoResponse.CreateBuilder();
            var channel = ChannelManager.GetChannelByEntityId(request.ChannelId);
            if (channel != null)
                builder.SetChannelInfo(channel.Info);
            else
                Logger.Warn("Channel does not exist!");

            done(builder.Build());
        }
    }
}
