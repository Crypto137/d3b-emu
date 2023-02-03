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
using System.Collections.Generic;
using D3BEmu.Common;
using D3BEmu.Common.Logging;

namespace D3BEmu.Core.EmuNet.Objects
{
    public static class RPCObjectManager
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        private static ulong _nextId=0;
        public static readonly Dictionary<ulong, RPCObject> Objects = new Dictionary<ulong, RPCObject>();

        static RPCObjectManager()
        { }

        public static void Init(RPCObject obj)
        {
            if (Objects.ContainsKey(obj.DynamicId))
                throw new Exception("Given object was already initialized");
            ulong id = Next();
            obj.DynamicId = id;
            Objects.Add(id, obj);
        }
        
        public static void Release(RPCObject obj)
        {
            Logger.Trace("Releasing object {0}", obj.DynamicId);
            Objects.Remove(obj.DynamicId);
        }
        
        public static ulong Next()
        {
            while (Objects.ContainsKey(++_nextId));
            return _nextId;
        }
    }
}
