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

using System.Collections.Generic;
using D3BEmu.Common;
using D3BEmu.Common.Logging;
using D3BEmu.Core.EmuNet.Helpers;
using D3BEmu.Net.EmuNet;

namespace D3BEmu.Core.EmuNet.Channels
{
    public static class ChannelManager
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        private readonly static Dictionary<ulong, Channel> Channels =
            new Dictionary<ulong, Channel>();

        public static Channel CreateNewChannel(EmuNetClient client, ulong remoteObjectId)
        {
            var channel = new Channel(client, remoteObjectId);
            Channels.Add(channel.DynamicId, channel);
            return channel;
        }

        public static void DissolveChannel(ulong id)
        {
            Logger.Debug("Dissolving channel {0}", id);
            if (!Channels.ContainsKey(id)) {
                Logger.Warn("Attempted to delete a non-existent channel with ID {0}", id);
                return;
            }
            var channel = Channels[id];
            channel.RemoveAllMembers(true);
            Channels.Remove(id);
        }

        public static Channel GetChannelByEntityId(bnet.protocol.EntityId entityId)
        {
            if (entityId.GetHighIdType() == EntityIdHelper.HighIdType.ChannelId)
            {
                if (Channels.ContainsKey(entityId.Low))
                    return Channels[entityId.Low];
            }
            else
            {
                Logger.Warn("Given entity ID doesn't look like a channel ID!");
            }
            return null;
        }
    }
}
