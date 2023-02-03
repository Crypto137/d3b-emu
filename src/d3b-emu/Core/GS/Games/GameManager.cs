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
using D3BEmu.Common.Extensions;
using D3BEmu.Common.Logging;
using D3BEmu.Core.GS.Players;
using D3BEmu.Core.EmuNet.Toons;
using D3BEmu.Net.GS.Message;

namespace D3BEmu.Core.GS.Games
{
    public static class GameManager
    {
        private static readonly Logger Logger = LogManager.CreateLogger();
        private static readonly Dictionary<int, Game> Games = new Dictionary<int, Game>();

        public static Game CreateGame(int gameId)
        {
            if (Games.ContainsKey(gameId))
                return Games[gameId];

            var game = new Game(gameId);
            Games.Add(gameId, game);
            return game;
        }

        public static Game GetGameById(int gameId)
        {
            return !Games.ContainsKey(gameId) ? null : Games[gameId];
        }

        public static void RemovePlayerFromGame(Net.GS.GameClient gameClient)
        {
            if (gameClient == null || gameClient.Game == null) return;

            var gameId = gameClient.Game.GameId;
            if (!Games.ContainsKey(gameId)) return;

            var game = Games[gameId];
            if (!game.Players.ContainsKey(gameClient)) return;

            Player p = null;
            if (!game.Players.TryRemove(gameClient, out p))
            {
                Logger.Error("Can't remove player ({0}) from game with id: {1}", gameClient.Player.Toon.Name, gameId);
            }

            if (p != null)
            {
                var toon = p.Toon;
                toon.ExperienceNext = p.Attributes[GameAttribute.Experience_Next];
                toon.TimePlayed += DateTimeExtensions.ToUnixTime(DateTime.UtcNow) - toon.LoginTime;
                toon.UnlockedStoneOfRecall = Convert.ToBoolean(p.Attributes[GameAttribute.Skill, 0x0002EC66]);
                toon.UnlockedCauldronOfJordan = p.Attributes[GameAttribute.ItemMeltUnlocked];
                toon.UnlockedNephalemCube = p.Attributes[GameAttribute.SalvageUnlocked];
                toon.SkillSet.Update(p.SkillSet);
                toon.SaveToDB();
                p.Inventory.SaveToDB();

                // Remove Player From World
                if (p.InGameClient != null)
                    p.World.Leave(p);
            }

            if (game.Players.Count == 0)
            {
                Games.Remove(gameId); // we should be also disposing it /raist.
            }
        }
    }
}
