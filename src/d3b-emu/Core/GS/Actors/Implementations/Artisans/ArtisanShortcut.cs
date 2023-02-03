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

using D3BEmu.Core.GS.Map;
using D3BEmu.Core.GS.Common.Types.TagMap;
using D3BEmu.Net.GS.Message;
using D3BEmu.Net.GS.Message.Definitions.Artisan;
using D3BEmu.Net.GS.Message.Definitions.World;
using D3BEmu.Core.GS.Players;

namespace D3BEmu.Core.GS.Actors.Implementations.Artisans
{
    [HandledSNO(0x0002FA63 /* PT_Blacksmith_ForgeWeaponShortcut.acr */,
        0x0002FA64 /*PT_Blacksmith_ForgeArmorShortcut.acr */,
        0x0002FA62 /*PT_Blacksmith_RepairShortcut.acr */, 
        212519 /* Actor PT_Jeweler_AddSocketShortcut */,
        212517 /* Actor PT_Jeweler_CombineShortcut */,
        212521 /* Actor PT_Jeweler_RemoveGemShortcut */,
        212511 /* Actor PT_Mystic_EnhanceShortcut */,
        212510 /* Actor PT_Mystic_IdentifyShortcut */)]
    public class ArtisanShortcut : InteractiveNPC
    {
        public ArtisanShortcut(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            Attributes[GameAttribute.MinimapActive] = false;
            Attributes[GameAttribute.Conversation_Icon, 0] = 0;
        }

        public override void OnTargeted(Player player, TargetMessage message)
        {
            player.InGameClient.SendMessage(new OpenArtisanWindowMessage() { ArtisanID = this.DynamicID });
        }
    }
}
