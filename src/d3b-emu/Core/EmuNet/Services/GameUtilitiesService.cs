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
using bnet.protocol.game_utilities;
using bnet.protocol.server_pool;
using D3BEmu.Common.Logging;

namespace D3BEmu.Core.EmuNet.Services
{
    [Service(serviceID: 0x8, serviceName: "bnet.protocol.game_utilities.GameUtilities")]
    public class GameUtilitiesService : GameUtilities,IServerService
    {
        private static readonly Logger Logger = LogManager.CreateLogger();
        public EmuNetClient Client { get; set; }

        public override void ProcessClientRequest(IRpcController controller, ClientRequest request, Action<ClientResponse> done)
        {
            var messageId = request.GetAttribute(1).Value.IntValue;
            Logger.Trace("ProcessClientRequest() ID: {0}", messageId);

            var builder = bnet.protocol.game_utilities.ClientResponse.CreateBuilder();
            var attr = bnet.protocol.attribute.Attribute.CreateBuilder();

            switch (messageId)
            {
                case 4:     //D3.GameMessages.SaveBannerConfiguration -> return MessageId with no Message
                    SaveBanner(D3.GameMessage.SaveBannerConfiguration.ParseFrom(request.GetAttribute(2).Value.MessageValue));
                    var attrId = bnet.protocol.attribute.Attribute.CreateBuilder()
                        .SetName("CustomMessageId")
                        .SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetIntValue(5).Build())
                        .Build();
                    builder.AddAttribute(attrId);
                    break;
                default:
                    Logger.Warn("Unknown CustomMessageId {0}: {1}", messageId, request.AttributeCount > 2 ? request.GetAttribute(2).Value.ToString() : "No CustomMessage?");
                    break;
            }

            if (attr.HasValue)
            {
                attr.SetName("CustomMessage");
                builder.AddAttribute(attr.Build());
            }

            done(builder.Build());
        }

        public override void CreateToon(IRpcController controller, CreateToonRequest request, Action<CreateToonResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void DeleteToon(IRpcController controller, DeleteToonRequest request, Action<NoData> done)
        {
            throw new NotImplementedException();
        }

        public override void TransferToon(IRpcController controller, TransferToonRequest request, Action<NoData> done)
        {
            throw new NotImplementedException();
        }

        public override void SelectToon(IRpcController controller, SelectToonRequest request, Action<NoData> done)
        {
            throw new NotImplementedException();
        }

        public bool SaveBanner(D3.GameMessage.SaveBannerConfiguration bannerConfig)
        {
            Logger.Trace("SaveBannerConfiguration()");

            if (this.Client.Account.BannerConfiguration == bannerConfig.Banner)
                return false;
            else
            {
                this.Client.Account.UpdateBanner(bannerConfig.Banner);
                //this.Client.Account.NotifyUpdate();   // NYI
            }
            return true;
        }

        public override void PresenceChannelCreated(IRpcController controller, PresenceChannelCreatedRequest request, Action<NoData> done)
        {
            throw new NotImplementedException();
        }

        public override void GetPlayerVariables(IRpcController controller, PlayerVariablesRequest request, Action<VariablesResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void GetGameVariables(IRpcController controller, GameVariablesRequest request, Action<VariablesResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void GetLoad(IRpcController controller, GetLoadRequest request, Action<ServerState> done)
        {
            throw new NotImplementedException();
        }
    }
}
