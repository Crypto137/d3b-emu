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

using D3BEmu.Common.Helpers;
using D3BEmu.Common.Helpers.Hash;
using D3BEmu.Core.EmuNet.Accounts;
using D3BEmu.Core.EmuNet.Toons;
using D3BEmu.Net.EmuNet;

namespace D3BEmu.Core.EmuNet.Commands
{
    // TODO: This a hackish way to enable server commands for players that are not in a party! As soon as we find a better way we should get rid of this /raist.

    /// <summary>
    /// CommandHandler account that server commands can be sent to.
    /// So with this we're able to trigger commands even we're not in party.
    /// </summary>
    public class CommandHandlerAccount:Account
    {
        private static readonly byte[] HandlerBanner = D3.Account.BannerConfiguration.CreateBuilder()
            .SetBackgroundColorIndex(2)
            .SetBannerIndex(0)
            .SetPattern(0)
            .SetPatternColorIndex(2)
            .SetPlacementIndex(0)
            .SetSigilAccent(8)
            .SetSigilMain(5)
            .SetSigilColorIndex(7)
            .SetUseSigilVariant(true)
            .Build().ToByteArray();

        private static readonly CommandHandlerAccount _instance = new CommandHandlerAccount();
        public static CommandHandlerAccount Instance { get { return _instance; } }

        private CommandHandlerAccount()
            : base(0, "server@d3b-emu", new byte[] {}, new byte[] {}, UserLevels.User, HandlerBanner, 0, 14, 0)
        {
            this.LoggedInClient = new CommandHandlerClient(this);
        }

        public void ParseCommand(bnet.protocol.notification.Notification request, EmuNetClient client)
        {
            if (request.Type != "WHISPER") return;
            if (request.AttributeCount <= 0 || !request.AttributeList[0].HasValue)  return;

            CommandManager.TryParse(request.AttributeList[0].Value.StringValue, client, CommandManager.RespondOver.Whisper);
        }
    }

    public class CommandHandlerClient : EmuNetClient
    {       
        public CommandHandlerClient(Account account):base(null)
        {
            this.Account = account;            
        }
    }

    public class CommandHandlerToon:Toon
    {
        private static readonly CommandHandlerToon _instance = new CommandHandlerToon();
        public static CommandHandlerToon Instance { get { return _instance; } }

        private CommandHandlerToon()
            : base("Server", 0, 0x1D4681B1, ToonFlags.Female, 99, CommandHandlerAccount.Instance)
        {
            this.Owner = CommandHandlerAccount.Instance;
            this.SetShinyEquipment(); // server needs shiny equipment! /raist.
        }

        public void SetShinyEquipment()
        {
            var visualItems = new[]
            {                                
                D3.Hero.VisualItem.CreateBuilder().SetGbid(0).SetDyeType(0).SetItemEffectType(0).SetEffectLevel(0).Build(), // Head
                D3.Hero.VisualItem.CreateBuilder().SetGbid(StringHashHelper.HashItemName("ChestArmor_203")).SetDyeType(0).SetItemEffectType(0).SetEffectLevel(0).Build(), // Chest
                D3.Hero.VisualItem.CreateBuilder().SetGbid(StringHashHelper.HashItemName("Boots_Unique_001")).SetDyeType(0).SetItemEffectType(0).SetEffectLevel(0).Build(), // Feet
                D3.Hero.VisualItem.CreateBuilder().SetGbid(StringHashHelper.HashItemName("Gloves_205")).SetDyeType(0).SetItemEffectType(0).SetEffectLevel(0).Build(), // Hands
                D3.Hero.VisualItem.CreateBuilder().SetGbid(StringHashHelper.HashItemName("Unique_Mighty_2H_001")).SetDyeType(0).SetItemEffectType(0).SetEffectLevel(0).Build(), // Weapon (1)
                D3.Hero.VisualItem.CreateBuilder().SetGbid(0).SetDyeType(0).SetItemEffectType(0).SetEffectLevel(0).Build(), // Weapon (2)
                D3.Hero.VisualItem.CreateBuilder().SetGbid(StringHashHelper.HashItemName("Shoulders_205")).SetDyeType(0).SetItemEffectType(0).SetEffectLevel(0).Build(), // Shoulders
                D3.Hero.VisualItem.CreateBuilder().SetGbid(0).SetDyeType(0).SetItemEffectType(0).SetEffectLevel(0).Build(), // Legs
            };

            this.Equipment = D3.Hero.VisualEquipment.CreateBuilder().AddRangeVisualItem(visualItems).Build();            
        }
    }
}
