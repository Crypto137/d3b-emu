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

using System.IO;
using D3BEmu.Common.Extensions;
using Google.ProtocolBuffers;

namespace D3BEmu.Net.EmuNet.Packets
{
    public class PacketOut
    {
        public byte ServiceId { get; private set; }
        public uint MethodId { get; private set; }
        public int RequestId { get; private set; }
        public ulong ObjectId { get; private set; }
        public IMessage Message { get; private set; }
        public byte[] Data { get; private set; }

        public PacketOut(byte serviceId, uint methodId, int requestId, IMessage message)
            : this(serviceId, methodId, requestId, 0x0, message)
        {
        }

        public PacketOut(byte serviceId, uint methodId, int requestId, ulong objectId, IMessage message)
        {
            this.ServiceId = serviceId;
            this.MethodId = methodId;
            this.RequestId = requestId;
            this.ObjectId = objectId;

            using (var stream = new MemoryStream())
            {
                var output = CodedOutputStream.CreateInstance(stream);

                output.WriteRawByte(serviceId);
                output.WriteInt32NoTag((int) methodId);
                output.WriteInt16NoTag((short) requestId);

                if (serviceId != 0xfe) output.WriteRawVarint64(this.ObjectId);

                this.Message = message;
                output.WriteMessageNoTag(message);
                output.Flush();
                this.Data = stream.ToArray();
            }
        }
    }
}
