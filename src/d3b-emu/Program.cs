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
using System.Globalization;
using System.Reflection;
using System.Threading;
using D3BEmu.Common;
using D3BEmu.Common.Logging;
using D3BEmu.Common.MPQ;
using D3BEmu.Core.GS.Items;
using D3BEmu.Core.EmuNet.Commands;
using D3BEmu.Net.GS;
using D3BEmu.Net.EmuNet;
using Environment = System.Environment;

namespace D3BEmu
{
    internal class Program
    {
        private static readonly Logger Logger = LogManager.CreateLogger();
        public static readonly DateTime StartupTime = DateTime.Now; 

        public static EmuNetServer EmuNetServer;
        public static GameServer GameServer;

        public static Thread EmuNetServerThread;
        public static Thread GameServerThread;

        public static void Main(string[] args)
        {
            // Watch for unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture; // Use invariant culture - we have to set it explicitly for every thread we create.

            Console.ForegroundColor = ConsoleColor.Yellow;
            PrintBanner();
            PrintLicense();
            Console.ResetColor();

            InitLoggers(); // init logging facility.

            Logger.Info("d3b-emu v{0} warming-up..", Assembly.GetExecutingAssembly().GetName().Version);

            if (!MPQStorage.Initialized)
            {
                Logger.Fatal("Cannot run servers as MPQStorage failed initialization.");
                Console.ReadLine();
                return;
            }

            Logger.Info("Item database loaded with a total of {0} item definitions.", ItemGenerator.TotalItems);

            StartupServers();
        }

        private static void InitLoggers()
        {
            LogManager.Enabled = true;

            foreach (var targetConfig in LogConfig.Instance.Targets)
            {
                if (!targetConfig.Enabled) continue;

                LogTarget target = null;
                switch (targetConfig.Target.ToLower())
                {
                    case "console":
                        target = new ConsoleTarget(targetConfig.MinimumLevel, targetConfig.MaximumLevel,
                                                   targetConfig.IncludeTimeStamps);
                        break;
                    case "file":
                        target = new FileTarget(targetConfig.FileName, targetConfig.MinimumLevel,
                                                targetConfig.MaximumLevel, targetConfig.IncludeTimeStamps,
                                                targetConfig.ResetOnStartup);
                        break;
                }

                if (target != null) LogManager.AttachLogTarget(target);
            }
        }

        private static void PrintBanner()
        {

            Console.WriteLine(@"      _ ____  _                               ");
            Console.WriteLine(@"     | |___ \| |                              ");
            Console.WriteLine(@"   __| | __) | |__ ______ ___ _ __ ___  _   _ ");
            Console.WriteLine(@"  / _` ||__ <| '_ \______/ _ \ '_ ` _ \| | | |");
            Console.WriteLine(@" | (_| |___) | |_) |    |  __/ | | | | | |_| |");
            Console.WriteLine(@"  \__,_|____/|_.__/      \___|_| |_| |_|\__,_|");
            Console.WriteLine();
        }

        private static void PrintLicense()
        {
            Console.WriteLine("Copyright (C) 2023 d3b-emu");
            Console.WriteLine("d3b-emu comes with ABSOLUTELY NO WARRANTY.");
            Console.WriteLine("This is free software, and you are welcome to redistribute it under certain conditions; see the LICENSE file for details.");
            Console.WriteLine();
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating)
                Logger.FatalException((e.ExceptionObject as Exception), "d3b-emu terminating because of unhandled exception.");                
            else
                Logger.ErrorException((e.ExceptionObject as Exception), "Caught unhandled exception.");
            Console.ReadLine();
        }

        #region server-control

        private static void StartupServers()
        {
            StartEmuNet();
            StartGS();

            while (true)
            {
                var line = Console.ReadLine();
                CommandManager.Parse(line);
            }
        }

        public static void Shutdown()
        {
            if (EmuNetServer != null)
            {
                Logger.Warn("Shutting down EmuNet-Server..");
                EmuNetServer.Shutdown();
            }

            if (GameServer != null)
            {
                Logger.Warn("Shutting down Game-Server..");
                GameServer.Shutdown();
            }

            Environment.Exit(0);
        }

        public static bool StartEmuNet()
        {
            if (EmuNetServer != null) return false;

            EmuNetServer = new EmuNetServer();
            EmuNetServerThread = new Thread(EmuNetServer.Run) {IsBackground = true, CurrentCulture = CultureInfo.InvariantCulture};
            EmuNetServerThread.Start();
            return true;
        }

        public static bool StopEmuNet()
        {
            if (EmuNetServer == null) return false;

            Logger.Warn("Stopping EmuNet-Server..");
            EmuNetServer.Shutdown();
            EmuNetServerThread.Abort();
            EmuNetServer = null;
            return true;
        }

        public static bool StartGS()
        {
            if (GameServer != null) return false;

            GameServer = new GameServer();
            GameServerThread = new Thread(GameServer.Run) { IsBackground = true, CurrentCulture = CultureInfo.InvariantCulture };
            GameServerThread.Start();
            return true;
        }

        public static bool StopGS()
        {
            if (GameServer == null) return false;

            Logger.Warn("Stopping Game-Server..");
            GameServer.Shutdown();
            GameServerThread.Abort();
            GameServer = null;
            return true;
        }

        #endregion
    }
}
