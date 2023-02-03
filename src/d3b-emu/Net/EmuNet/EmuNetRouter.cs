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
using System.Linq;
using Google.ProtocolBuffers;
using Google.ProtocolBuffers.Descriptors;
using D3BEmu.Common;
using D3BEmu.Common.Logging;
using D3BEmu.Core.EmuNet.Services;
using D3BEmu.Net.EmuNet.Packets;

namespace D3BEmu.Net.EmuNet
{
    public static class EmuNetRouter
    {
        private static readonly Logger Logger = LogManager.CreateLogger();
        public const byte ServiceReply = 0xFE;

        public static void Route(ConnectionDataEventArgs e)
        {
            var stream = CodedInputStream.CreateInstance(e.Data.ToArray());
            while (!stream.IsAtEnd)
            {
                Identify(e.Connection, stream);
            }
        }

        public static void Identify(IConnection connection, CodedInputStream stream)
        {
            var client = (EmuNetClient) connection.Client;
            var packet = new PacketIn(stream);
                  
            if(packet.ServiceId==ServiceReply)
            {
                var callback = client.RPCCallbacks.Dequeue();
                
                if (callback.RequestId == packet.RequestId) callback.Action(packet.ReadMessage(callback.Builder));
                else Logger.Warn("RPC callback contains unexpected requestId: {0} where {1} was expected", callback.RequestId, packet.RequestId);
                return;
            }
            
            var service = Service.GetByID(packet.ServiceId);

            if (service == null)
            {
                Logger.Error("No service exists with id: 0x{0}", packet.ServiceId.ToString("X2"));
                return;
            }

            var method = service.DescriptorForType.Methods.Single(m => GetMethodId(m) == packet.MethodId);
            var proto = service.GetRequestPrototype(method);
            var message = packet.ReadMessage(proto.WeakToBuilder());

            try
            {
                lock (service) // lock the service so that its in-context client does not get changed..
                {
                    ((IServerService)service).Client = client;
                    service.CallMethod(method, null, message, (msg => SendRPCResponse(connection, packet.RequestId, msg)));
                }
            }
            catch (NotImplementedException)
            {
                Logger.Warn("Unimplemented service method: {0}.{1}", service.GetType().Name, method.Name);
            }
            catch (UninitializedMessageException e)
            {
                Logger.Debug("Failed to parse message: {0}", e.Message);
            }
            catch (Exception e)
            {
                Logger.DebugException(e, string.Empty);
            }
        }

        public static uint GetMethodId(MethodDescriptor method)
        {
            return (uint)method.Options[bnet.protocol.Rpc.MethodId.Descriptor];
        }

        private static void SendRPCResponse(IConnection connection, int requestId, IMessage message)
        {
            var packet = new PacketOut(ServiceReply, 0x0, requestId, message);
            connection.Send(packet);
        }
    }
}
