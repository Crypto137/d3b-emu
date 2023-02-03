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
using System.Linq;
using System.Text;
using D3BEmu.Net.GS.Message.Fields;
using D3BEmu.Core.GS.Players;
using D3BEmu.Core.GS.Actors.Implementations.Hirelings;

namespace D3BEmu.Core.GS.Actors.Interactions
{
    public interface IInteraction
    {
        NPCInteraction AsNPCInteraction(InteractiveNPC npc, Player player);
    }

    public class ConversationInteraction : IInteraction
    {
        public int ConversationSNO;
        public bool Read;

        public ConversationInteraction(int conversationSNO)
        {
            ConversationSNO = conversationSNO;
            Read = false; // Should read from players saved data /fasbat
        }

        public NPCInteraction AsNPCInteraction(InteractiveNPC npc, Player player)
        {
            return new NPCInteraction()
            {
                Type = NPCInteractionType.Conversation,
                ConversationSNO = this.ConversationSNO,
                Field2 = -1,
                State = (Read ? NPCInteractionState.Used : NPCInteractionState.New),
            };
        }

        public void MarkAsRead() // Just a hack to show functionality /fasbat
        {
            Read = true;
        }
    }

    public class HireInteraction : IInteraction
    {
        public NPCInteraction AsNPCInteraction(InteractiveNPC npc, Player player)
        {
            return new NPCInteraction()
            {
                Type = NPCInteractionType.Hire,
                ConversationSNO = -1,
                Field2 = -1,
                State = (npc as Hireling).HasHireling ? NPCInteractionState.New : NPCInteractionState.Disabled
            };
        }
    }

    public class IdentifyAllInteraction : IInteraction
    {
        public NPCInteraction AsNPCInteraction(InteractiveNPC npc, Player player)
        {
            return new NPCInteraction()
            {
                Type = NPCInteractionType.IdentifyAll,
                ConversationSNO = -1,
                Field2 = -1,
                State = NPCInteractionState.New // Has items to identify? If no disable,
            };
        }
    }

    public class CraftInteraction : IInteraction
    {
        public NPCInteraction AsNPCInteraction(InteractiveNPC npc, Player player)
        {
            return new NPCInteraction()
            {
                Type = NPCInteractionType.Craft,
                ConversationSNO = -1,
                Field2 = -1,
                State = NPCInteractionState.New,
            };
        }
    }

    public class InventoryInteraction : IInteraction
    {
        public NPCInteraction AsNPCInteraction(InteractiveNPC npc, Player player)
        {
            return new NPCInteraction()
            {
                Type = NPCInteractionType.Inventory,
                ConversationSNO = -1,
                Field2 = -1,
                State = (npc as Hireling).HasProxy ? NPCInteractionState.New : NPCInteractionState.Disabled
            };
        }
    }
   
}
