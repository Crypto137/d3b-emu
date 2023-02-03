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

using D3BEmu.Common;
using D3BEmu.Common.Logging;
using D3BEmu.Core.EmuNet.Accounts;
using D3BEmu.Core.EmuNet.Helpers;
using D3BEmu.Core.EmuNet.Toons;
using D3BEmu.Net.EmuNet;

// TODO: Need to do some more testing and inspection to make sure that
// responding before performing the action requested is proper

namespace D3BEmu.Core.EmuNet.Services
{
    [Service(serviceID: 0xb, serviceName: "bnet.protocol.presence.PresenceService")]
    public class PresenceService : bnet.protocol.presence.PresenceService,IServerService
    {
        private static readonly Logger Logger = LogManager.CreateLogger();
        public EmuNetClient Client { get; set; }

        public override void Subscribe(Google.ProtocolBuffers.IRpcController controller, bnet.protocol.presence.SubscribeRequest request, System.Action<bnet.protocol.NoData> done)
        {
            switch (request.EntityId.GetHighIdType())
            {
                case EntityIdHelper.HighIdType.AccountId:
                    var account = AccountManager.GetAccountByPersistentID(request.EntityId.Low);
                    if (account != null)
                    {
                        Logger.Trace("Subscribe() {0} {1}", this.Client, account);
                        account.AddSubscriber(this.Client, request.ObjectId);
                    }
                    break;
                case EntityIdHelper.HighIdType.ToonId:
                    var toon = ToonManager.GetToonByLowID(request.EntityId.Low);                    
                    if (toon != null) 
                    {
                        Logger.Trace("Subscribe() {0} {1}", this.Client, toon);
                        toon.AddSubscriber(this.Client, request.ObjectId); // The client will send us a Subscribe with ToonId of 0 the first time it tries to create a toon with a name that already exists. Let's handle that here.
                    }
                    break;
                default:
                    Logger.Warn("Recieved an unhandled Presence.Subscribe request with type {0}", request.EntityId.GetHighIdType());
                    break;
            }

            var builder = bnet.protocol.NoData.CreateBuilder();
            done(builder.Build());
        }

        public override void Unsubscribe(Google.ProtocolBuffers.IRpcController controller, bnet.protocol.presence.UnsubscribeRequest request, System.Action<bnet.protocol.NoData> done)
        {
            Logger.Trace("Unsubscribe()");
            
            switch (request.EntityId.GetHighIdType())
            {
                case EntityIdHelper.HighIdType.AccountId:
                    var account = AccountManager.GetAccountByPersistentID(request.EntityId.Low);
                    // The client will probably make sure it doesn't unsubscribe to a null ID, but just to make sure..
                    if (account != null)
                    {
                        account.RemoveSubscriber(this.Client);
                        Logger.Trace("Unsubscribe() {0} {1}", this.Client, account);
                    }
                    break;
                case EntityIdHelper.HighIdType.ToonId:
                    var toon = ToonManager.GetToonByLowID(request.EntityId.Low);
                    if (toon != null)
                    {
                        toon.RemoveSubscriber(this.Client);
                        Logger.Trace("Unsubscribe() {0} {1}", this.Client, toon);
                    }
                    break;
                default:
                    Logger.Warn("Recieved an unhandled Presence.Unsubscribe request with type {0}", request.EntityId.GetHighIdType());
                    break;
            }
            
            var builder = bnet.protocol.NoData.CreateBuilder();
            done(builder.Build());
        }

        public override void Update(Google.ProtocolBuffers.IRpcController controller, bnet.protocol.presence.UpdateRequest request, System.Action<bnet.protocol.NoData> done)
        {            
            //Logger.Warn("request:\n{0}", request.ToString());
            // This "UpdateRequest" is not, as it may seem, a request to update the client on the state of an object,
            // but instead the *client* requesting to change fields on an object that it has subscribed to.
            // Check docs/rpc/presence.txt in branch wip-docs (or master)

            switch (request.EntityId.GetHighIdType())
            {
                case EntityIdHelper.HighIdType.AccountId:
                    var account = AccountManager.GetAccountByPersistentID(request.EntityId.Low);
                    Logger.Trace("Update() {0} {1}", this.Client, account);
                    break;
                case EntityIdHelper.HighIdType.ToonId:
                    var toon = ToonManager.GetToonByLowID(request.EntityId.Low);
                    Logger.Trace("Update() {0} {1}", this.Client, toon);
                    foreach(var fieldOp in request.FieldOperationList)
                    {
                        // Check if toon is not null to prevent errors when switching heroes
                        if (toon == null)
                        {
                            Logger.Warn("Failed to update toon: is null");
                        }
                        else
                        {
                            toon.Update(fieldOp);
                        }  
                    }
                    break;
                default:
                    Logger.Warn("Recieved an unhandled Presence.Update request with type {0}", request.EntityId.GetHighIdType());
                    break;
            }

            var builder = bnet.protocol.NoData.CreateBuilder();
            done(builder.Build());
        }

        public override void Query(Google.ProtocolBuffers.IRpcController controller, bnet.protocol.presence.QueryRequest request, System.Action<bnet.protocol.presence.QueryResponse> done)
        {            
            var builder = bnet.protocol.presence.QueryResponse.CreateBuilder();

            switch (request.EntityId.GetHighIdType())
            {
                case EntityIdHelper.HighIdType.AccountId:
                    var account = AccountManager.GetAccountByPersistentID(request.EntityId.Low);
                    Logger.Trace("Query() {0} {1}", this.Client, account);
                    foreach(var key in request.KeyList)
                    {
                        var field = account.QueryField(key);
                        if (field != null) builder.AddField(field);
                    }

                    break;
                case EntityIdHelper.HighIdType.ToonId:
                    var toon = ToonManager.GetToonByLowID(request.EntityId.Low);
                    Logger.Trace("Query() {0} {1}", this.Client, toon);                    
                    foreach (var key in request.KeyList)
                    {
                        var field = toon.QueryField(key);
                        if (field != null) builder.AddField(field);
                    }

                    break;
            }

            done(builder.Build());
        }               
    }
}
