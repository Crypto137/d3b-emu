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

namespace D3BEmu.Core.EmuNet.Commands
{
    public sealed class Config: D3BEmu.Common.Config.Config
    {
        public char CommandPrefix { get { return this.GetString("CommandPrefix", "!")[0]; } set { this.Set("CommandPrefix", value); } }

        private static readonly Config _instance = new Config();
        public static Config Instance { get { return _instance; } }
        private Config() : base("Commands") { }
    }
}
