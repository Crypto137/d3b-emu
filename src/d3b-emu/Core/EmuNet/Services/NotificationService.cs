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
using D3BEmu.Core.EmuNet.Commands;
using D3BEmu.Core.EmuNet.Helpers;
using D3BEmu.Core.EmuNet.Toons;
using D3BEmu.Net.EmuNet;

namespace D3BEmu.Core.EmuNet.Services
{
    [Service(serviceID: 0xc, serviceName: "bnet.protocol.notification.NotificationService")]
    public class NotificationService : bnet.protocol.notification.NotificationService, IServerService
    {
        private static readonly Logger Logger = LogManager.CreateLogger();
        public EmuNetClient Client { get; set; }

        public override void SendNotification(Google.ProtocolBuffers.IRpcController controller, bnet.protocol.notification.Notification request, Action<bnet.protocol.NoData> done)
        {
            switch (request.GetNotificationType())
            {
                case NotificationTypeHelper.NotificationType.Whisper:
                    
                    // NOTE: Real implementation doesn't even handle the situation where neither client knows about the other.
                    // Client requires prior knowledge of sender and target (and even then it cannot whisper by using the /whisper command).

                    Logger.Trace(string.Format("NotificationRequest.Whisper by {0} to {1}", this.Client.CurrentToon, ToonManager.GetToonByLowID(request.TargetId.Low)));

                    var account = ToonManager.GetOwnerAccountByToonLowId(request.TargetId.Low);
                    if (account.LoggedInClient == null) return;

                    if (account is CommandHandlerAccount) // hackish way to enable server commands for players that are not in party.
                        CommandHandlerAccount.Instance.ParseCommand(request, this.Client);
                    else
                    {
                        var notification = bnet.protocol.notification.Notification.CreateBuilder(request)
                            .SetSenderId(this.Client.CurrentToon.BnetEntityID)
                            .Build();

                        account.LoggedInClient.MakeRPC(() => bnet.protocol.notification.NotificationListener.CreateStub(account.LoggedInClient).
                                      OnNotificationReceived(controller, notification, callback => { }));
                    }
                    break;
                default:
                    Logger.Warn("Unhandled notification type: {0}", request.Type);
                    break;
            }

            var builder = bnet.protocol.NoData.CreateBuilder();
            done(builder.Build());
        }

        public override void RegisterClient(Google.ProtocolBuffers.IRpcController controller, bnet.protocol.notification.RegisterClientRequest request, Action<bnet.protocol.NoData> done)
        {
            throw new NotImplementedException();
        }

        public override void UnregisterClient(Google.ProtocolBuffers.IRpcController controller, bnet.protocol.notification.UnregisterClientRequest request, Action<bnet.protocol.NoData> done)
        {
            throw new NotImplementedException();
        }

        public override void FindClient(Google.ProtocolBuffers.IRpcController controller, bnet.protocol.notification.FindClientRequest request, Action<bnet.protocol.notification.FindClientResponse> done)
        {
            throw new NotImplementedException();
        }
    }
}
