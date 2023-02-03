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
using D3BEmu.Net.EmuNet;

namespace D3BEmu.Core.EmuNet.Online
{
    // probably will not need this when we actually send players from last game to recent players window.
    public static class PlayerManager
    {
        public static readonly List<EmuNetClient> OnlinePlayers = new List<EmuNetClient>();

        public static void PlayerConnected(EmuNetClient client)
        {
            OnlinePlayers.Add(client);
        }

        public static void PlayerDisconnected(EmuNetClient client)
        {
            OnlinePlayers.Remove(client);
        }
    }
}
