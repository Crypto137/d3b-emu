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

using System.IO;
using System.Collections.Generic;
using Gibbed.IO;
using D3BEmu.Common;
using D3BEmu.Net.EmuNet;
using D3BEmu.Common.Logging;

namespace D3BEmu.Core.EmuNet.Services
{
    [Service(serviceID: 0x9, serviceName: "bnet.protocol.storage.StorageService")]
    public class StorageService : bnet.protocol.storage.StorageService,IServerService
    {
        private static readonly Logger Logger = LogManager.CreateLogger();
        public EmuNetClient Client { get; set; }

        public override void OpenTable(Google.ProtocolBuffers.IRpcController controller, bnet.protocol.storage.OpenTableRequest request, System.Action<bnet.protocol.storage.OpenTableResponse> done)
        {
            Logger.Trace("OpenTable() {0}", this.Client);
            var builder = bnet.protocol.storage.OpenTableResponse.CreateBuilder();
            done(builder.Build());
        }

        public override void OpenColumn(Google.ProtocolBuffers.IRpcController controller, bnet.protocol.storage.OpenColumnRequest request, System.Action<bnet.protocol.storage.OpenColumnResponse> done)
        {
            Logger.Trace("OpenColumn() {0}", this.Client);
            var builder = bnet.protocol.storage.OpenColumnResponse.CreateBuilder();
            done(builder.Build());
        }

        public override void Execute(Google.ProtocolBuffers.IRpcController controller, bnet.protocol.storage.ExecuteRequest request, System.Action<bnet.protocol.storage.ExecuteResponse> done)
        {
            Logger.Trace("Execute() {0}", this.Client);
            bnet.protocol.storage.ExecuteResponse response = null;
            switch (request.QueryName)
            {
                case "GetGameAccountSettings":
                    response = GameAccountSettings(request);
                    break;
                case "LoadAccountDigest":
                    response = LoadAccountDigest(Client, request);
                    break;
                case "GetHeroDigests":
                    response = GetHeroDigest(Client, request);
                    break;
                case "GetToonSettings":
                    response = GetToonSettings(request);
                    break;
                default:
                    Logger.Warn("Unhandled query: {0}", request.QueryName);
                    response = bnet.protocol.storage.ExecuteResponse.CreateBuilder().Build();
                    break;
            }

            done(response);
        }

        private bnet.protocol.storage.ExecuteResponse GetHeroDigest(EmuNetClient client, bnet.protocol.storage.ExecuteRequest request)
        {
            var results = new List<bnet.protocol.storage.OperationResult>();

            foreach(var operation in request.OperationsList)
            {
                // find the requested toons entity-id.
                var stream = new MemoryStream(operation.RowId.Hash.ToByteArray());
                
                // contains ToonHandle in field form with one unknown field (which is not in message definition):
                // int16 unknown; uint8 realm; uint8 region; uint32 program; uint64 id;
                stream.ReadValueU16(); // unknown
                stream.ReadValueU8(); // realm
                stream.ReadValueU8(); // region 
                stream.ReadValueU32(false); // program
                    
                var toonId=stream.ReadValueU64(false);

                if(!client.Account.Toons.ContainsKey(toonId))
                {
                    Logger.Error("Can't find the requested toon: {0}", toonId);
                    continue;
                }

                var toon = client.Account.Toons[toonId];                    
                var operationResult = bnet.protocol.storage.OperationResult.CreateBuilder().SetTableId(operation.TableId);
                operationResult.AddData(
                    bnet.protocol.storage.Cell.CreateBuilder()
                        .SetColumnId(request.OperationsList[0].ColumnId)
                        .SetRowId(request.OperationsList[0].RowId)
                        .SetVersion(1)
                        .SetData(toon.Digest.ToByteString())
                        .Build()
                    );
                results.Add(operationResult.Build());
            }

            var builder = bnet.protocol.storage.ExecuteResponse.CreateBuilder();            
            foreach(var result in results)
            {
                builder.AddResults(result);
            }            
            return builder.Build();
        }

        private bnet.protocol.storage.ExecuteResponse GetToonSettings(bnet.protocol.storage.ExecuteRequest request)
        {
            var results = new List<bnet.protocol.storage.OperationResult>();

            foreach (var operation in request.OperationsList)
            {
                var operationResult = bnet.protocol.storage.OperationResult.CreateBuilder().SetTableId(operation.TableId);
                operationResult.AddData(
                    bnet.protocol.storage.Cell.CreateBuilder()
                        .SetColumnId(request.OperationsList[0].ColumnId)
                        .SetRowId(request.OperationsList[0].RowId)
                        .SetVersion(1)
                        .Build()
                    );
                results.Add(operationResult.Build());
            }

            var builder = bnet.protocol.storage.ExecuteResponse.CreateBuilder();
            foreach (var result in results)
            {
                builder.AddResults(result);
            }
            return builder.Build();
        }

        private bnet.protocol.storage.ExecuteResponse LoadAccountDigest(EmuNetClient client, bnet.protocol.storage.ExecuteRequest request)
        {
            var results = new List<bnet.protocol.storage.OperationResult>();          

            foreach (var operation in request.OperationsList)
            {
                var operationResult = bnet.protocol.storage.OperationResult.CreateBuilder().SetTableId(operation.TableId);
                operationResult.AddData(
                    bnet.protocol.storage.Cell.CreateBuilder()
                        .SetColumnId(request.OperationsList[0].ColumnId)
                        .SetRowId(request.OperationsList[0].RowId)
                        .SetVersion(1)
                        .SetData(client.Account.Digest.ToByteString())
                        .Build());
                results.Add(operationResult.Build());
            }

            var builder = bnet.protocol.storage.ExecuteResponse.CreateBuilder();
            foreach (var result in results)
            {
                builder.AddResults(result);
            }
            return builder.Build();
        }

        private bnet.protocol.storage.ExecuteResponse GameAccountSettings(bnet.protocol.storage.ExecuteRequest request)
        {
            var results = new List<bnet.protocol.storage.OperationResult>();

            foreach (var operation in request.OperationsList)
            {
                var operationResult = bnet.protocol.storage.OperationResult.CreateBuilder().SetTableId(operation.TableId);
                operationResult.AddData(
                    bnet.protocol.storage.Cell.CreateBuilder()
                        .SetColumnId(request.OperationsList[0].ColumnId)
                        .SetRowId(request.OperationsList[0].RowId)
                        .Build());
                results.Add(operationResult.Build());
            }

            var builder = bnet.protocol.storage.ExecuteResponse.CreateBuilder();
            foreach (var result in results)
            {
                builder.AddResults(result);
            }
            return builder.Build();
        }
    }
}
