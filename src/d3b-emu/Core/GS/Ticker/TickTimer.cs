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

using System;
using D3BEmu.Core.GS.Games;
using D3BEmu.Core.GS.Objects;

namespace D3BEmu.Core.GS.Ticker
{
    /// <summary>
    /// A stepped tick timer that can fire a completion action on timeout.
    /// </summary>
    public class TickTimer : IUpdateable
    {
        /// <summary>
        /// Exact tick value to timeout.
        /// </summary>
        public int TimeoutTick { get; private set; }

        /// <summary>
        /// The game timer belongs to.
        /// </summary>
        public Game Game { get; private set; }

        /// <summary>
        /// The completition action to be called on timeout.
        /// </summary>
        public Action<int> CompletionAction { get; private set; }

        /// <summary>
        /// Creates a new tick timer that can fire a completition action timeout.
        /// </summary>
        /// <param name="game">The game timer belongs to</param>
        /// <param name="timeoutTick">Exact tick value to timeout</param>
        /// <param name="completionCallback">The completition action to be called on timeout</param>
        public TickTimer(Game game, int timeoutTick, Action<int> completionCallback = null)
        {
            // why is this even here? it causes problems for me /MDZ
            //if (timeoutTick <= game.TickCounter)
            //    throw new ArgumentOutOfRangeException("timeoutTick", string.Format("timeoutTick value {0} can not be equal or less then timer's belonging game's current TickCounter value {1}.", timeoutTick, game.TickCounter));
                                                          
            this.Game = game;
            this.TimeoutTick = timeoutTick;
            this.CompletionAction = completionCallback;
        }

        /// <summary>
        /// Returns true if the timer is timed-out.
        /// </summary>
        public bool TimedOut
        {
            get { return Game.TickCounter >= TimeoutTick; }
        }

        /// <summary>
        /// Returns true if timer is still running.
        /// </summary>
        public bool Running
        {
            get { return !this.TimedOut; }
        }

        /// <summary>
        /// Updates the timer.
        /// </summary>
        /// <param name="tickCounter">The current tick-counter.</param>
        public virtual void Update(int tickCounter)
        {
            if (this.TimeoutTick == -1) // means timer is already fired there.
                return;

            if (!this.TimedOut) // if we haven't timed-out yet, return.
                return;

            if (this.CompletionAction != null) // if a completition action exists.
                this.CompletionAction(tickCounter); //call it once the timer time-outs.

            this.Stop();
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public void Stop()
        {
            this.TimeoutTick = -1;
        }

        /// <summary>
        /// Creates a new tick timer.
        /// </summary>
        /// <param name="game">The game timer belongs to.</param>
        /// <param name="ticks">Relative tick amount taken to timeout.</param>
        /// <param name="completionCallback">The completition action to be called on timeout.</param>
        /// <returns><see cref="SteppedTickTimer"/></returns>
        public static TickTimer WaitTicks(Game game, int ticks, Action<int> completionCallback = null)
        {
            return new RelativeTickTimer(game, ticks, completionCallback);
        }

        /// <summary>
        /// Creates a new seconds based tick timer.
        /// </summary>
        /// <param name="game">The game timer belongs to.</param>
        /// <param name="seconds">Seconds taken to timeout.</param>
        /// <param name="completionCallback">The completition action to be called on timeout.</param>
        /// <returns><see cref="SteppedTickTimer"/></returns>
        public static TickTimer WaitSeconds(Game game, float seconds, Action<int> completionCallback = null)
        {
            return new SecondsTickTimer(game, seconds, completionCallback);
        }

        /// <summary>
        /// Creates a new mili-seconds based tick timer.
        /// </summary>
        /// <param name="game">The game timer belongs to.</param>
        /// <param name="miliSeconds">MiliSeconds taken to timeout.</param>
        /// <param name="completionCallback">The completition action to be called on timeout.</param>
        /// <returns><see cref="SteppedTickTimer"/></returns>
        public static TickTimer WaitMiliSeconds(Game game, float miliSeconds, Action<int> completionCallback = null)
        {
            return new MiliSecondsTickTimer(game, miliSeconds, completionCallback);
        }
    }

    /// <summary>
    /// Relative tick timer.
    /// </summary>
    public class RelativeTickTimer : TickTimer
    {
        /// <summary>
        /// Creates a new relative tick timer.
        /// </summary>
        /// <param name="game">The game timer belongs to.</param>
        /// <param name="ticks">Relative tick amount taken to timeout.</param>
        /// <param name="completionCallback">The completition action to be called on timeout.</param>
        public RelativeTickTimer(Game game, int ticks, Action<int> completionCallback = null)
            : base(game, game.TickCounter + ticks, completionCallback)
        { }
    }

    /// <summary>
    /// Seconds based tick timer.
    /// </summary>
    public class SecondsTickTimer : RelativeTickTimer
    {
        /// <summary>
        /// Creates a new seconds based tick timer.
        /// </summary>
        /// <param name="game">The game timer belongs to.</param>
        /// <param name="seconds">Seconds taken to timeout.</param>
        /// <param name="completionCallback">The completition action to be called on timeout.</param>
        /// <returns><see cref="SteppedTickTimer"/></returns>
        public SecondsTickTimer(Game game, float seconds, Action<int> completionCallback = null)
            : base(game, (int)(1000f / game.UpdateFrequency * game.TickRate * seconds), completionCallback)
        { }
    }

    /// <summary>
    /// Mili-seconds based tick timer.
    /// </summary>
    public class MiliSecondsTickTimer : RelativeTickTimer
    {
        /// <summary>
        /// Creates a new mili-seconds based tick timer.
        /// </summary>
        /// <param name="game">The game timer belongs to.</param>
        /// <param name="miliSeconds">MiliSeconds taken to timeout.</param>
        /// <param name="completionCallback">The completition action to be called on timeout.</param>
        /// <returns><see cref="SteppedTickTimer"/></returns>
        public MiliSecondsTickTimer(Game game, float miliSeconds, Action<int> completionCallback = null)
            : base(game, (int)((1000f / game.UpdateFrequency * game.TickRate) / 1000f * miliSeconds), completionCallback)
        { }
    }
}