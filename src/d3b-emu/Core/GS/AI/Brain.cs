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

using System.Collections.Generic;
using D3BEmu.Common.Logging;
using D3BEmu.Core.GS.Actors;
using D3BEmu.Core.GS.Actors.Actions;

namespace D3BEmu.Core.GS.AI
{
    public class Brain
    {
        protected static readonly Logger Logger = LogManager.CreateLogger();

        /// <summary>
        /// The body chained to brain.
        /// </summary>
        public Actor Body { get; private set; }

        /// <summary>
        /// The current brain state.
        /// </summary>
        public BrainState State { get; protected set; }

        /// <summary>
        /// The current action for the brain.
        /// </summary>
        public ActorAction CurrentAction { get; protected set; }

        /// <summary>
        /// Actions to be taken.
        /// </summary>
        public Queue<ActorAction> Actions { get; protected set; }

        protected Brain(Actor body)
        {
            this.Body = body;
            this.State = BrainState.Idle;
            this.Actions = new Queue<ActorAction>();
        }

        protected void QueueAction(ActorAction action)
        {
            this.Actions.Enqueue(action);
        }

        public virtual void Update(int tickCounter)
        {
            if (this.State == BrainState.Dead || this.Body == null || this.Body.World == null || this.State == BrainState.Off)
                return;

            this.Think(tickCounter); // let the brain think.
            this.Perform(tickCounter); // perform any outstanding actions.
        }

        /// <summary>
        /// Lets the brain think and decide the next action to take.
        /// </summary>
        public virtual void Think(int tickCounter)
        { }

        /// <summary>
        /// Stop all brain activities.
        /// </summary>
        public virtual void Kill()
        {
            if (this.CurrentAction != null)
            {
                this.CurrentAction.Cancel(0);
                this.CurrentAction = null;
            }
            this.State = BrainState.Dead;
        }

        public void Activate()
        {
            if (this.State == BrainState.Off)
                this.State = BrainState.Idle;
        }

        public void DeActivate()
        {
            this.State = BrainState.Off;
        }

        private void Perform(int tickCounter)
        {
            if (this.CurrentAction == null)
                return;

            if (!this.CurrentAction.Started)
                this.CurrentAction.Start(tickCounter);
            else
                this.CurrentAction.Update(tickCounter);

            if (this.CurrentAction.Done)
                this.CurrentAction = null;
        }
    }
}
