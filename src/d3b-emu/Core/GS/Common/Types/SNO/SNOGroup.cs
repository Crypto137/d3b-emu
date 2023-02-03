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

namespace D3BEmu.Core.GS.Common.Types.SNO
{
    public enum SNOGroup : int
    {
        Code = -2,
        None = -1,
        Actor = 1,
        Adventure = 2,
        AiBehavior = 3,
        AiState = 4,
        AmbientSound = 5,
        Anim = 6,
        Anim2D = 7,
        AnimSet = 8,
        Appearance = 9,
        Hero = 10,
        Cloth = 11,
        Conversation = 12,
        ConversationList = 13,
        EffectGroup = 14,
        Encounter = 15,
        Explosion = 17,
        FlagSet = 18,
        Font = 19,
        GameBalance = 20,
        Globals = 21,
        LevelArea = 22,
        Light = 23,
        MarkerSet = 24,
        Monster = 25,
        Observer = 26,
        Particle = 27,
        Physics = 28,
        Power = 29,
        Quest = 31,
        Rope = 32,
        Scene = 33,
        SceneGroup = 34,
        Script = 35,
        ShaderMap = 36,
        Shaders = 37,
        Shakes = 38,
        SkillKit = 39,
        Sound = 40,
        SoundBank = 41,
        StringList = 42,
        Surface = 43,
        Textures = 44,
        Trail = 45,
        UI = 46,
        Weather = 47,
        Worlds = 48,
        Recipe = 49,
        Condition = 51,
        TreasureClass = 52,
        Account = 53,
        Conductor = 54,
        TimedEvent = 55,
        Act = 56,
        Material = 57,
        QuestRange = 58,
        Lore = 59,
        Reverb = 60,
        PhysMesh = 61,
        Music = 62,
        Tutorial = 63,
        BossEncounter = 64,
    }
}
