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
using System.Net;
using System.Net.Sockets;
using Google.ProtocolBuffers;
using Google.ProtocolBuffers.Descriptors;
using D3BEmu.Common;
using D3BEmu.Common.Logging;
using D3BEmu.Net.EmuNet.Packets;

namespace D3BEmu.Net
{
    public class Connection : IConnection
    {
        protected static readonly Logger Logger = LogManager.CreateLogger();

        private readonly Server _server;
        private readonly Socket _socket;
        private readonly byte[] _recvBuffer = new byte[BufferSize];
        public static readonly int BufferSize = 16 * 1024; // 16 KB       

        public IClient Client { get; set; }

        public Connection(Server server, Socket socket)
        {
            if (server == null) throw new ArgumentNullException("server");
            if (socket == null) throw new ArgumentNullException("socket");

            this._server = server;
            this._socket = socket;
        }       

        #region socket stuff

        public bool IsConnected
        {
            get { return _socket.Connected; }
        }

        public IPEndPoint RemoteEndPoint
        {
            get { return _socket.RemoteEndPoint as IPEndPoint; }
        }

        public IPEndPoint LocalEndPoint
        {
            get { return _socket.LocalEndPoint as IPEndPoint; }
        }

        public byte[] RecvBuffer
        {
            get { return _recvBuffer; }
        }

        public Socket Socket
        {
            get { return _socket; }
        }

        public IAsyncResult BeginReceive(AsyncCallback callback, object state)
        {
            return _socket.BeginReceive(_recvBuffer, 0, BufferSize, SocketFlags.None, callback, state);
        }

        public int EndReceive(IAsyncResult result)
        {
            return _socket.EndReceive(result);
        }

        public int Send(PacketOut packet)
        {
            if (packet == null) throw new ArgumentNullException("packet");
            return Send(packet.Data);
        }

        public int Send(IEnumerable<byte> data)
        {
            if (data == null) throw new ArgumentNullException("data");
            return Send(data, SocketFlags.None);
        }

        public int Send(IEnumerable<byte> data, SocketFlags flags)
        {
            if (data == null) throw new ArgumentNullException("data");
            return _server.Send(this, data, flags);
        }

        public int Send(byte[] buffer)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            return Send(buffer, 0, buffer.Length, SocketFlags.None);
        }

        public int Send(byte[] buffer, SocketFlags flags)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            return Send(buffer, 0, buffer.Length, flags);
        }

        public int Send(byte[] buffer, int start, int count)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            return Send(buffer, start, count, SocketFlags.None);
        }

        public int Send(byte[] buffer, int start, int count, SocketFlags flags)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            return _server.Send(this, buffer, start, count, flags);
        }

        public void Disconnect()
        {
            if (this.IsConnected)
                _server.Disconnect(this);
        }

        public override string ToString()
        {
            if (_socket.RemoteEndPoint != null)
                return _socket.RemoteEndPoint.ToString();
            else
                return "Not Connected";
        }

        #endregion
    }
}
