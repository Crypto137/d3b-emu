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
using D3BEmu.Common;
using D3BEmu.Common.Logging;
using D3BEmu.Net.EmuNet;

namespace D3BEmu.Core.EmuNet.Services
{
    [Service(serviceID: 0x4, serviceName: "bnet.protocol.followers.FollowersService")]
    public class FollowersService : bnet.protocol.followers.FollowersService,IServerService
    {
        private static readonly Logger Logger = LogManager.CreateLogger();
        public EmuNetClient Client { get; set; }

        public override void SubscribeToFollowers(Google.ProtocolBuffers.IRpcController controller, bnet.protocol.followers.SubscribeToFollowersRequest request, System.Action<bnet.protocol.followers.SubscribeToFollowersResponse> done)
        {
            Logger.Trace("Subscribe() {0}", this.Client);

            var builder = bnet.protocol.followers.SubscribeToFollowersResponse.CreateBuilder();
            done(builder.Build());
        }

        public override void StartFollowing(Google.ProtocolBuffers.IRpcController controller, bnet.protocol.followers.StartFollowingRequest request, System.Action<bnet.protocol.followers.StartFollowingResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void StopFollowing(Google.ProtocolBuffers.IRpcController controller, bnet.protocol.followers.StopFollowingRequest request, System.Action<bnet.protocol.followers.StopFollowingResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void UpdateFollowerState(Google.ProtocolBuffers.IRpcController controller, bnet.protocol.followers.UpdateFollowerStateRequest request, System.Action<bnet.protocol.followers.UpdateFollowerStateResponse> done)
        {
            throw new NotImplementedException();
        }
    }
}
