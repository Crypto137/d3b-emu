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

using System.Linq;
using D3BEmu.Common;
using D3BEmu.Core.GS.Players;
using D3BEmu.Core.GS.Map;
using D3BEmu.Net.GS.Message.Definitions.World;
using D3BEmu.Core.GS.Common.Types.SNO;
using D3BEmu.Core.GS.Markers;
using D3BEmu.Common.MPQ.FileFormats.Types;
using D3BEmu.Core.GS.Common.Types.TagMap;
using D3BEmu.Common.Logging;

namespace D3BEmu.Core.GS.Items.Implementations
{
    [HandledType("Book")]
    public class Book : Item
    {
        public static readonly Logger Logger = LogManager.CreateLogger();

        public int LoreSNOId { get; private set; }

        public Book(World world, D3BEmu.Common.MPQ.FileFormats.ItemTable definition)
            : base(world, definition)
        {
            // Items are NOT constructed with tags
            var actorData = ActorSNO.Target as D3BEmu.Common.MPQ.FileFormats.Actor;

            if (actorData.TagMap.ContainsKey(ActorKeys.Lore))
            {
                LoreSNOId = actorData.TagMap[ActorKeys.Lore].Id;
            }
        }

/*
  // Items are NOT constructed with tags!
        protected override void ReadTags()
        {
            base.ReadTags();
            if (this.Tags.ContainsKey((int)MarkerTagTypes.LoreSNOId))
            {
                LoreSNOId = Tags[(int)MarkerTagTypes.LoreSNOId].Int2;
            }
            else
            {
                LoreSNOId = -1;
            }
        }
*/
        public override void OnTargeted(Player player, TargetMessage message)
        {
            //Logger.Trace("OnTargeted");
            if (LoreSNOId != -1)
            {
                player.PlayLore(LoreSNOId, true);
            }
            if (player.GroundItems.ContainsKey(this.DynamicID))
                player.GroundItems.Remove(this.DynamicID);
            this.Destroy();
        }
    }
}
