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
using D3BEmu.Net.GS.Message.Definitions.Misc;

namespace D3BEmu.Core.GS.Actors.Implementations
{
    class Savepoint : Gizmo
    {
        private bool _savepointReached = false;

        public int SavepointId { get; private set; }

        public Savepoint(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            SavepointId = tags[MarkerKeys.SavepointId];
        }

        public override void OnPlayerApproaching(Players.Player player)
        {
            if (player.Position.DistanceSquared(ref _position) < ActorData.Sphere.Radius * ActorData.Sphere.Radius * this.Scale * this.Scale && !_savepointReached)
            {
                _savepointReached = true;

                // TODO send real SavePointInformation, though im not sure if that is used for anything at all
                this.World.BroadcastIfRevealed(new SavePointInfoMessage
                {
                    snoLevelArea = 102362,
                }, this);

                player.SavePointData = new Net.GS.Message.Fields.SavePointData() { snoWorld = World.WorldSNO.Id, SavepointId = SavepointId };
                player.UpdateHeroState();
                player.CheckPointPosition = this._position; // This seemed easier than having on Death find the SavePoint based on ID, then getting its location. - DarkLotus
            }
        }
    }
}
