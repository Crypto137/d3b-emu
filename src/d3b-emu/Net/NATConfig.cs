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

using D3BEmu.Common.Config;

namespace D3BEmu.Net
{
    public sealed class NATConfig: Config
    {
        public bool Enabled { get { return this.GetBoolean("Enabled", true); } set { this.Set("Enabled", value); } }
        public string PublicIP { get { return this.GetString("PublicIP", "0.0.0.0"); } set { this.Set("PublicIP", value); } }

        private static readonly NATConfig _instance = new NATConfig();
        public static NATConfig Instance { get { return _instance; } }
        private NATConfig() : base("NAT") { }
    }
}
