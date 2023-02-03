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

namespace D3BEmu.Common.Storage
{
    public sealed class Config : Common.Config.Config
    {
        public string Root { get { return this.GetString("Root", "Assets"); } set { this.Set("Root", value); } }
        public string MPQRoot { get { return this.GetString("MPQRoot", "Assets/MPQ"); } set { this.Set("MPQRoot", value); } }
        public bool EnableTasks { get { return this.GetBoolean("EnableTasks", true); } set { this.Set("EnableTasks", value); } }
        
        private static readonly Config _instance = new Config();
        public static Config Instance { get { return _instance; } }
        private Config() : base("Storage") { }
    }
}
