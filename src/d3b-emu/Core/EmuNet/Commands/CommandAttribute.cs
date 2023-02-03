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
using D3BEmu.Core.EmuNet.Accounts;

namespace D3BEmu.Core.EmuNet.Commands
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandGroupAttribute : Attribute
    {
        /// <summary>
        /// Command group's name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Help text for command group.
        /// </summary>
        public string Help { get; private set; }

        /// <summary>
        /// Minimum user level required to invoke the command.
        /// </summary>
        public Account.UserLevels MinUserLevel { get; private set; }

        public CommandGroupAttribute(string name, string help, Account.UserLevels minUserLevel = Account.UserLevels.User)
        {
            this.Name = name.ToLower();
            this.Help = help;
            this.MinUserLevel = minUserLevel;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        /// <summary>
        /// Command's name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Help text for command.
        /// </summary>
        public string Help { get; private set; }

        /// <summary>
        /// Minimum user level required to invoke the command.
        /// </summary>
        public Account.UserLevels MinUserLevel { get; private set; }

        public CommandAttribute(string command, string help, Account.UserLevels minUserLevel = Account.UserLevels.User)
        {
            this.Name = command.ToLower();
            this.Help = help;
            this.MinUserLevel = minUserLevel;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class DefaultCommand : CommandAttribute
    {
        public DefaultCommand(Account.UserLevels minUserLevel = Account.UserLevels.User)
            : base("", "", minUserLevel)
        {
        }
    }
}
