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
using D3BEmu.Core.EmuNet.Online;
using D3BEmu.Net.EmuNet;
using bnet.protocol.user_manager;
using D3BEmu.Common.Logging;

namespace D3BEmu.Core.EmuNet.Services
{
    [Service(serviceID: 0x5, serviceName: "bnet.protocol.user_manager.UserManagerService")]
    public class UserManagerService : bnet.protocol.user_manager.UserManagerService,IServerService
    {
        private static readonly Logger Logger = LogManager.CreateLogger();
        public EmuNetClient Client { get; set; }

        public override void SubscribeToUserManager(Google.ProtocolBuffers.IRpcController controller, bnet.protocol.user_manager.SubscribeToUserManagerRequest request, System.Action<bnet.protocol.user_manager.SubscribeToUserManagerResponse> done)
        {
            Logger.Trace("Subscribe() {0}", this.Client);

            // temp hack: send him all online players on server where he should be normally get list of player he met in his last few games /raist.

            var builder = SubscribeToUserManagerResponse.CreateBuilder();
            foreach (var client in PlayerManager.OnlinePlayers)
            {
                if (client == this.Client) continue; // Don't add the requester to the list                
                if (client.CurrentToon == null) continue;

                Logger.Debug("RecentPlayer => " + client.CurrentToon);
                var recentPlayer = RecentPlayer.CreateBuilder().SetPlayer(client.CurrentToon.BnetEntityID);
                builder.AddRecentPlayers(recentPlayer);
            }

            done(builder.Build());
        }

        public override void ReportPlayer(IRpcController controller, ReportPlayerRequest request, Action<ReportPlayerResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void BlockPlayer(IRpcController controller, BlockPlayerRequest request, Action<BlockPlayerResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void RemovePlayerBlock(IRpcController controller, RemovePlayerBlockRequest request, Action<RemovePlayerBlockResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void AddRecentPlayers(IRpcController controller, AddRecentPlayersRequest request, Action<AddRecentPlayersResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void RemoveRecentPlayers(IRpcController controller, RemoveRecentPlayersRequest request, Action<RemoveRecentPlayersResponse> done)
        {
            throw new NotImplementedException();
        }
    }
}
