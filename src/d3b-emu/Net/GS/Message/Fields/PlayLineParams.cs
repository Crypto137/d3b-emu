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

using System.Text;

namespace D3BEmu.Net.GS.Message.Fields
{
    public enum VoiceGender
    {
        Male = 0,
        Female = 1,
    }

    public enum Class
    {
        None = -1,
        DemonHunter = 0,
        Barbarian = 1,
        Wizard = 2,
        WitchDoctor = 3,
        Monk = 4
    }

    public class PlayLineParams
    {
        public int SNOConversation;
        public int Field1;          // have not seen != 0
        public bool Field2;         // have not seen true
        public int LineID;          // the ID of the line within the conversation
                                    // Participant to speak out? must mach what the lineID is expecting... eg. LineID == 6 expects 2 while LineID == 5 expects 1 (in a specific dialogue)
                                    // Set to 0 for a conversation line said by the player - farmy
        public int Speaker;         // this could be the same as in Conversation-mpq filetype
        public int Field5;          // have not seen != -1
        public Class TextClass;     // Class enum used to pick a class specific text. Or -1 for npc text
        public VoiceGender Gender;  // Used if Field4 set to 0, (Use hero's gender) audio
        public Class AudioClass;    // Used if Field4 set to 0, (Use hero's class) audio
        public int SNOSpeakerActor; // Picture of this actor is used on the text (if any)
        public string Name;         // Name of the actor if Field4 is set to 0 ("Hero speaking")
        public int Field11;
        public int AnimationTag;
        public int Duration;
        public int Field14;         // seems to be a running number across conversationlines. StopConvLine.Field0 == EndConvLine.Field0 == PlayConvLine.PlayLineParams.Field14 for a conversation
        public int Field15;

        public void Parse(GameBitBuffer buffer)
        {
            SNOConversation = buffer.ReadInt(32);
            Field1 = buffer.ReadInt(32);
            Field2 = buffer.ReadBool();
            LineID = buffer.ReadInt(32);
            Speaker = buffer.ReadInt(32);
            Field5 = buffer.ReadInt(32);
            TextClass = (Class)buffer.ReadInt(32);
            Gender = (VoiceGender)buffer.ReadInt(32);
            AudioClass = (Class)buffer.ReadInt(32);
            SNOSpeakerActor = buffer.ReadInt(32);
            Name = buffer.ReadCharArray(49);
            Field11 = buffer.ReadInt(32);
            AnimationTag = buffer.ReadInt(32);
            Duration = buffer.ReadInt(32);
            Field14 = buffer.ReadInt(32);
            Field15 = buffer.ReadInt(32);
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, SNOConversation);
            buffer.WriteInt(32, Field1);
            buffer.WriteBool(Field2);
            buffer.WriteInt(32, LineID);
            buffer.WriteInt(32, Speaker);
            buffer.WriteInt(32, Field5);
            buffer.WriteInt(32, (int)TextClass);
            buffer.WriteInt(32, (int)Gender);
            buffer.WriteInt(32, (int)AudioClass);
            buffer.WriteInt(32, SNOSpeakerActor);
            buffer.WriteCharArray(49, Name);
            buffer.WriteInt(32, Field11);
            buffer.WriteInt(32, AnimationTag);
            buffer.WriteInt(32, Duration);
            buffer.WriteInt(32, Field14);
            buffer.WriteInt(32, Field15);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PlayLineParams:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("snoConversation: 0x" + SNOConversation.ToString("X8"));
            b.Append(' ', pad);
            b.AppendLine("Field1: 0x" + Field1.ToString("X8") + " (" + Field1 + ")");
            b.Append(' ', pad);
            b.AppendLine("Field2: " + (Field2 ? "true" : "false"));
            b.Append(' ', pad);
            b.AppendLine("LineID: 0x" + LineID.ToString("X8") + " (" + LineID + ")");
            b.Append(' ', pad);
            b.AppendLine("Speaker: 0x" + Speaker.ToString("X8") + " (" + Speaker + ")");
            b.Append(' ', pad);
            b.AppendLine("Field5: 0x" + Field5.ToString("X8") + " (" + Field5 + ")");
            b.Append(' ', pad);
            b.AppendLine("TextClass: 0x" + ((int)TextClass).ToString("X8") + " (" + TextClass + ")");
            b.Append(' ', pad);
            b.AppendLine("Gender: 0x" + ((int)Gender).ToString("X8") + " (" + Gender + ")");
            b.Append(' ', pad);
            b.AppendLine("AudioClass: 0x" + ((int)AudioClass).ToString("X8") + " (" + AudioClass + ")");
            b.Append(' ', pad);
            b.AppendLine("snoSpeakerActor: 0x" + SNOSpeakerActor.ToString("X8"));
            b.Append(' ', pad);
            b.AppendLine("Name: \"" + Name + "\"");
            b.Append(' ', pad);
            b.AppendLine("Field11: 0x" + Field11.ToString("X8") + " (" + Field11 + ")");
            b.Append(' ', pad);
            b.AppendLine("AnimationTag: 0x" + AnimationTag.ToString("X8") + " (" + AnimationTag + ")");
            b.Append(' ', pad);
            b.AppendLine("Duration: 0x" + Duration.ToString("X8") + " (" + Duration + ")");
            b.Append(' ', pad);
            b.AppendLine("Field14: 0x" + Field14.ToString("X8") + " (" + Field14 + ")");
            b.Append(' ', pad);
            b.AppendLine("Field15: 0x" + Field15.ToString("X8") + " (" + Field15 + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}