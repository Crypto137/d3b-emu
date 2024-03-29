﻿/*
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

using D3BEmu.Common;
using D3BEmu.Common.Logging;

namespace D3BEmu
{
    public sealed class LogConfig : Common.Config.Config
    {
        public string LoggingRoot { get { return this.GetString("Root", @"logs"); } set { this.Set("Root", value); } }

        public LogTargetConfig[] Targets = new[] { new LogTargetConfig("ConsoleLog"), new LogTargetConfig("ServerLog"), new LogTargetConfig("PacketLog") };

        private static readonly LogConfig _instance = new LogConfig();
        public static LogConfig Instance { get { return _instance; } }
        private LogConfig() : base("Logging") { }
    }

    public class LogTargetConfig : Common.Config.Config
    {
        public bool Enabled { get { return this.GetBoolean("Enabled", true); } set { this.Set("Enabled", value); } }
        public string Target { get { return this.GetString("Target", "Console"); } set { this.GetString("Target", value); } }
        public bool IncludeTimeStamps { get { return this.GetBoolean("IncludeTimeStamps", false); } set { this.Set("IncludeTimeStamps", value); } }
        public string FileName { get { return this.GetString("FileName", ""); } set { this.GetString("FileName", value); } }
        public Logger.Level MinimumLevel { get { return (Logger.Level)( this.GetInt("MinimumLevel", (int)Logger.Level.Info, true)); } set { this.Set("MinimumLevel", (int)value ); } }
        public Logger.Level MaximumLevel { get { return (Logger.Level)(this.GetInt("MaximumLevel", (int)Logger.Level.Fatal, true)); } set { this.Set("MaximumLevel", (int)value); } }      
        public bool ResetOnStartup { get { return this.GetBoolean("ResetOnStartup", false); } set { this.Set("ResetOnStartup", value); } }

        public LogTargetConfig(string loggerName) : base(loggerName) { }
    }
}
