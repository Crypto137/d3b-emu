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
using System.Reflection;
using D3BEmu.Common.Logging;

namespace D3BEmu.Core.GS.Powers
{
    public class PowerLoader
    {
        static readonly Logger Logger = LogManager.CreateLogger();

        private static Dictionary<int, Type> _implementations = new Dictionary<int, Type>();

        public static PowerScript CreateImplementationForPowerSNO(int powerSNO)
        {
            if (_implementations.ContainsKey(powerSNO))
            {
                PowerScript script = (PowerScript)Activator.CreateInstance(_implementations[powerSNO]);
                script.PowerSNO = powerSNO;
                return script;
            }
            else
            {
                Logger.Debug("Unimplemented power: {0}", powerSNO);
                return null;
            }
        }

        public static bool HasImplementationForPowerSNO(int powerSNO)
        {
            return _implementations.ContainsKey(powerSNO);
        }

        static PowerLoader()
        {
            // Find all subclasses of PowerImplementation and index them by the PowerSNO they are attributed with.
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsSubclassOf(typeof(PowerScript)))
                {
                    var attributes = (ImplementsPowerSNO[])type.GetCustomAttributes(typeof(ImplementsPowerSNO), true);
                    foreach (var powerAttribute in attributes)
                    {
                        _implementations[powerAttribute.PowerSNO] = type;
                    }
                }
            }
        }
    }
}
