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

using D3BEmu.Core.GS.Map;
using D3BEmu.Core.GS.Common.Types.TagMap;
using D3BEmu.Net.GS.Message.Definitions.Effect;

namespace D3BEmu.Core.GS.Actors.Implementations
{
    /// <summary>
    /// Implementation of checkpoints. Do they have any other purpose than sending the checkpoint visual?
    /// </summary>
    class Checkpoint : Gizmo
    {
        private bool _checkpointReached = false;

        public Checkpoint(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {

        }

        public override void OnPlayerApproaching(Players.Player player)
        {
            if (player.Position.DistanceSquared(ref _position) < ActorData.Sphere.Radius * ActorData.Sphere.Radius * this.Scale * this.Scale && !_checkpointReached)
            {
                _checkpointReached = true;

                this.World.BroadcastIfRevealed(new PlayEffectMessage
                {
                    ActorId = player.DynamicID,
                    Effect = Effect.Checkpoint
                }, this);
            }
        }

    }
}
