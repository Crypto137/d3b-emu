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

using D3BEmu.Common.Logging;

namespace D3BEmu.Core.GS.Actors.Actions
{
    public abstract class ActorAction
    {
        protected static readonly Logger Logger = LogManager.CreateLogger();

        /// <summary>
        /// The action owner actor.
        /// </summary>
        public Actor Owner { get; private set; }

        /// <summary>
        /// Returns true if the action is completed.
        /// </summary>
        public bool Done { get; protected set; }

        /// <summary>
        /// Returns true if the action is already started.
        /// </summary>
        public bool Started { get; protected set; }

        protected ActorAction(Actor owner)
        {
            this.Owner = owner;
            this.Started = false;
            this.Done = false;
        }

        public abstract void Start(int tickCounter);

        public abstract void Update(int tickCounter);

        public abstract void Cancel(int tickCounter);
    }
}
