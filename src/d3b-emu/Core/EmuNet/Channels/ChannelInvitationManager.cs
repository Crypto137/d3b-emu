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

using System.Collections.Generic;
using System.Linq;
using D3BEmu.Core.EmuNet.Objects;
using D3BEmu.Core.EmuNet.Toons;
using D3BEmu.Net.EmuNet;

namespace D3BEmu.Core.EmuNet.Channels
{
    public class ChannelInvitationManager : RPCObject
    {
        private readonly Dictionary<ulong, bnet.protocol.invitation.Invitation> _onGoingInvitations = new Dictionary<ulong, bnet.protocol.invitation.Invitation>();

        public static ulong InvitationIdCounter = 1;

        public void HandleInvitation(EmuNetClient client, bnet.protocol.invitation.Invitation invitation)
        {
            var invitee = this.Subscribers.FirstOrDefault(subscriber => subscriber.CurrentToon.BnetEntityID.Low == invitation.InviteeIdentity.ToonId.Low);
            if (invitee == null) return; // if we can't find invite just return - though we should actually check for it until expiration time.

            this._onGoingInvitations.Add(invitation.Id, invitation); // track ongoing invitations so we can tranport it forth and back.

            var notification = bnet.protocol.channel_invitation.InvitationAddedNotification.CreateBuilder().SetInvitation(invitation);                      

            invitee.MakeTargetedRPC(this,() =>
                bnet.protocol.channel_invitation.ChannelInvitationNotify.CreateStub(invitee).NotifyReceivedInvitationAdded(null, notification.Build(), callback => { }));
        }

        public void HandleAccept(EmuNetClient client, bnet.protocol.channel_invitation.AcceptInvitationRequest request)
        {
            if (!this._onGoingInvitations.ContainsKey(request.InvitationId)) return;
            var invitation = this._onGoingInvitations[request.InvitationId];
            var channel = ChannelManager.GetChannelByEntityId(invitation.GetExtension(bnet.protocol.channel_invitation.Invitation.ChannelInvitation).ChannelDescription.ChannelId);

            channel.Join(client, request.ObjectId); // add invitee to channel -- so inviter and other members will also be notified too.

            var notification =
                bnet.protocol.channel_invitation.InvitationRemovedNotification.CreateBuilder().SetInvitation(invitation)
                    .SetReason((uint) InvitationRemoveReason.Accepted);

            this._onGoingInvitations.Remove(invitation.Id);

            // notify invitee and let him remove the handled invitation.
            client.MakeTargetedRPC(this,() =>
                bnet.protocol.channel_invitation.ChannelInvitationNotify.CreateStub(client).NotifyReceivedInvitationRemoved(null, notification.Build(), callback => { }));
        }

        public void HandleDecline(EmuNetClient client, bnet.protocol.invitation.GenericRequest request)
        {
            if (!this._onGoingInvitations.ContainsKey(request.InvitationId)) return;
            var invitation = this._onGoingInvitations[request.InvitationId];

            var inviter = ToonManager.GetToonByLowID(invitation.InviterIdentity.ToonId.Low);
            if (inviter == null || inviter.Owner.LoggedInClient == null) return;

            var notification =
                bnet.protocol.channel.UpdateChannelStateNotification.CreateBuilder()
                .SetAgentId(bnet.protocol.EntityId.CreateBuilder().SetHigh(0).SetLow(0)) // caps have this set to high: 0 low: 0 /raist.
                .SetStateChange(bnet.protocol.channel.ChannelState.CreateBuilder().AddInvitation(invitation)
                .SetReason((uint) InvitationRemoveReason.Declined));

            this._onGoingInvitations.Remove(invitation.Id);

            // notify invoker about the decline.
            inviter.Owner.LoggedInClient.MakeTargetedRPC(inviter.Owner.LoggedInClient.CurrentChannel, () =>
                bnet.protocol.channel.ChannelSubscriber.CreateStub(inviter.Owner.LoggedInClient).NotifyUpdateChannelState(null, notification.Build(), callback => { }));
        }

        public void Revoke(EmuNetClient client, bnet.protocol.channel_invitation.RevokeInvitationRequest request)
        {
            if (!this._onGoingInvitations.ContainsKey(request.InvitationId)) return;
            var invitation = this._onGoingInvitations[request.InvitationId];

            //notify inviter about revoke
            var updateChannelNotification =
                bnet.protocol.channel.UpdateChannelStateNotification.CreateBuilder()
                .SetAgentId(bnet.protocol.EntityId.CreateBuilder().SetHigh(0).SetLow(0)) // caps have this set to high: 0 low: 0 /dustin
                .SetStateChange(bnet.protocol.channel.ChannelState.CreateBuilder()
                    .AddInvitation(invitation)
                    .SetReason((uint)InvitationRemoveReason.Revoked));

            this._onGoingInvitations.Remove(request.InvitationId);

            client.MakeTargetedRPC(client.CurrentChannel, () =>
                bnet.protocol.channel.ChannelSubscriber.CreateStub(client).NotifyUpdateChannelState(null, updateChannelNotification.Build(), callback => { }));

            //notify invitee about revoke
            var invitationRemoved =
                bnet.protocol.channel_invitation.InvitationRemovedNotification.CreateBuilder()
                .SetInvitation(invitation)
                .SetReason((uint)InvitationRemoveReason.Revoked);

            var invitee = ToonManager.GetToonByLowID(invitation.InviteeIdentity.ToonId.Low);
            invitee.Owner.LoggedInClient.MakeTargetedRPC(this, () =>
                bnet.protocol.channel_invitation.ChannelInvitationNotify.CreateStub(invitee.Owner.LoggedInClient).NotifyReceivedInvitationRemoved(null, invitationRemoved.Build(), callback => { }));
        }

        public enum InvitationRemoveReason : uint // not sure -- and don't have all the values yet /raist.
        {
            Accepted = 0x0,
            Declined = 0x1,
            Revoked = 0x2
        }
    }
}
