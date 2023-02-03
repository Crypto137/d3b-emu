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
using D3BEmu.Net.GS.Message.Definitions.Animation;
using D3BEmu.Net.GS.Message;
using D3BEmu.Core.GS.Ticker;

namespace D3BEmu.Core.GS.Actors.Implementations
{
    class ProximityTriggeredGizmo : Gizmo
    {
        private bool _collapsed = false;

        public ProximityTriggeredGizmo(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
        }

        public override void OnPlayerApproaching(Players.Player player)
        {
            if (player.Position.DistanceSquared(ref _position) < ActorData.Sphere.Radius * ActorData.Sphere.Radius * this.Scale * this.Scale && !_collapsed)
            {
                _collapsed = true;

                // TODO most of the fields here are unknown, find out about animation playing duration
                int duration = 500; // ticks
                World.BroadcastIfRevealed(new PlayAnimationMessage
                {
                    ActorID = this.DynamicID,
                    Field1 = 11,
                    Field2 = 0,
                    tAnim = new Net.GS.Message.Fields.PlayAnimationMessageSpec[]
                    {
                        new Net.GS.Message.Fields.PlayAnimationMessageSpec()
                        {
                            Duration = duration,
                            AnimationSNO = ActorData.TagMap.ContainsKey(ActorKeys.DeathAnimationTag) ? AnimationSet.TagMapAnimDefault[ActorData.TagMap[ActorKeys.DeathAnimationTag]].Int : AnimationSet.TagMapAnimDefault[AnimationSetKeys.DeathDefault] ,
                            PermutationIndex = 0,
                            Speed = 1
                        }
                    }

                }, this);

                World.BroadcastIfRevealed(new SetIdleAnimationMessage
                {
                    ActorID = this.DynamicID,
                    AnimationSNO = AnimationSetKeys.DeadDefault.ID
                }, this);

                this.Attributes[GameAttribute.Deleted_On_Server] = true;
                Attributes.BroadcastChangedIfRevealed();

                RelativeTickTimer destroy = new RelativeTickTimer(World.Game, duration, x => this.Destroy());
            }
        }

    }
}
