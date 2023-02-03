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
using D3BEmu.Common;
using D3BEmu.Common.Logging;
using D3BEmu.Core.EmuNet.Channels;
using D3BEmu.Core.EmuNet.Commands;
using D3BEmu.Net.EmuNet;

namespace D3BEmu.Core.EmuNet.Services
{
    [Service(serviceID: 0x10, serviceName: "bnet.protocol.channel.Channel")]
    public class ChannelService : bnet.protocol.channel.Channel, IServerService
    {
        private static readonly Logger Logger = LogManager.CreateLogger();
        public EmuNetClient Client { get; set; }

        public override void AddMember(Google.ProtocolBuffers.IRpcController controller, bnet.protocol.channel.AddMemberRequest request, System.Action<bnet.protocol.NoData> done)
        {
            throw new NotImplementedException();
        }

        public override void Dissolve(Google.ProtocolBuffers.IRpcController controller, bnet.protocol.channel.DissolveRequest request, System.Action<bnet.protocol.NoData> done)
        {
            throw new NotImplementedException();
        }

        public override void RemoveMember(Google.ProtocolBuffers.IRpcController controller, bnet.protocol.channel.RemoveMemberRequest request, System.Action<bnet.protocol.NoData> done)
        {
            Logger.Trace("RemoveMember()");

            // TODO: we should be actually checking for which member has to be removed. /raist.            
            var builder = bnet.protocol.NoData.CreateBuilder();
            done(builder.Build());
            this.Client.CurrentChannel.RemoveMember(this.Client, Channel.GetRemoveReasonForRequest((Channel.RemoveRequestReason)request.Reason));
        }

        public override void SendMessage(Google.ProtocolBuffers.IRpcController controller, bnet.protocol.channel.SendMessageRequest request, System.Action<bnet.protocol.NoData> done)
        {
            Logger.Trace("{0} sent a message to channel {1}.", this.Client.CurrentToon, this.Client.CurrentChannel);

            var builder = bnet.protocol.NoData.CreateBuilder();
            done(builder.Build());

            if (!request.HasMessage) return; // only continue if the request actually contains a message.

            if (!(request.Message.AttributeCount > 0 && request.Message.AttributeList[0].HasValue &&
                CommandManager.TryParse(request.Message.AttributeList[0].Value.StringValue, this.Client, CommandManager.RespondOver.Channel))) // try parsing the message as a command   
            {
                this.Client.CurrentChannel.SendMessage(this.Client, request.Message); // if it's not - let channel itself to broadcast message to it's members.  
            }
        }

        public override void SetRoles(Google.ProtocolBuffers.IRpcController controller, bnet.protocol.channel.SetRolesRequest request, System.Action<bnet.protocol.NoData> done)
        {
            throw new NotImplementedException();
        }

        public override void UpdateChannelState(Google.ProtocolBuffers.IRpcController controller, bnet.protocol.channel.UpdateChannelStateRequest request, System.Action<bnet.protocol.NoData> done)
        {
            var channel = Client.CurrentChannel;
            Logger.Trace("UpdateChannelState(): {0}", channel.ToString());

            // TODO: Should be actually applying changes on channel. /raist.
            var channelState = bnet.protocol.channel.ChannelState.CreateBuilder();

            foreach (bnet.protocol.attribute.Attribute attribute in request.StateChange.AttributeList)
            {
                if (attribute.Name == "D3.Party.GameCreateParams")
                {
                    if (attribute.HasValue && !attribute.Value.MessageValue.IsEmpty) //Sometimes not present -Egris
                    {
                        var gameCreateParams = D3.OnlineService.GameCreateParams.ParseFrom(attribute.Value.MessageValue);

                        //if (channel.Attributes.ContainsKey(attribute.Name))
                        //    channel.Attributes[attribute.Name] = attribute;
                        //else
                        //    channel.Attributes.Add(attribute.Name, attribute);
                        
                        var attr = bnet.protocol.attribute.Attribute.CreateBuilder()
                            .SetName("D3.Party.GameCreateParams")
                            .SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetMessageValue(gameCreateParams.ToByteString()).Build());
                        channelState.AddAttribute(attr);

                        if (gameCreateParams.Coop.SnoQuest == 87700 && gameCreateParams.Coop.QuestStepId == -1)
                        {
                            Client.CurrentToon.CurrentQuest = -1;
                            channel.Owner.CurrentToon.CurrentQuestStepId = 0;
                        }
                        else
                        {
                            Client.CurrentToon.CurrentQuest = gameCreateParams.Coop.SnoQuest;
                            Client.CurrentToon.CurrentQuestStepId = gameCreateParams.Coop.QuestStepId;
                        }

                        Logger.Trace("D3.Party.GameCreateParams: {0}", gameCreateParams.ToString());
                    }
                }
                else
                {
                    Logger.Warn("UpdateChannelState(): Unknown attribute: {0}", attribute.Name);
                }
            }

            var builder = bnet.protocol.NoData.CreateBuilder();
            done(builder.Build());

            if (request.StateChange.HasPrivacyLevel)
                channelState.PrivacyLevel = request.StateChange.PrivacyLevel;

            var notification = bnet.protocol.channel.UpdateChannelStateNotification.CreateBuilder()
                .SetAgentId(this.Client.Account.BnetAccountID)
                .SetStateChange(channelState)
                .Build();

            //Notify all Channel members
            foreach (var member in channel.Members.Keys)
            {
                member.MakeTargetedRPC(channel, () =>
                    bnet.protocol.channel.ChannelSubscriber.CreateStub(member).NotifyUpdateChannelState(null, notification, callback => { }));
            }
        }

        public override void UpdateMemberState(Google.ProtocolBuffers.IRpcController controller, bnet.protocol.channel.UpdateMemberStateRequest request, System.Action<bnet.protocol.NoData> done)
        {
            throw new NotImplementedException();
        }
    }
}
