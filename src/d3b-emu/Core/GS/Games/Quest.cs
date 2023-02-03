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
using D3BEmu.Net.GS.Message.Definitions.Quest;
using D3BEmu.Core.GS.Common.Types.SNO;

namespace D3BEmu.Core.GS.Games
{
    public interface QuestProgressHandler
    {
        void Notify(D3BEmu.Common.MPQ.FileFormats.QuestStepObjectiveType type, int value);
    }

    public class Quest : QuestProgressHandler
    {
        public const int ProtectorOfTristram = 87700;   // The Fallen Star
        public const int RescueCain = 72095;            // The Legacy of Cain
        public const int Blacksmith = 72221;            // A Shattered Crown
        public const int SkeletonKing = 72061;          // Reign of the Black King

        /// <summary>
        /// Keeps track of a single quest step
        /// </summary>
        public class QuestStep : QuestProgressHandler
        {
            /// <summary>
            /// Keeps track of a single quest step objective
            /// </summary>
            public class QuestObjective : QuestProgressHandler
            {
                public int Counter { get; private set; }
                public bool Done { get { return (objective.CounterTarget == 0 && Counter > 0) || Counter == objective.CounterTarget; } }
                public int ID { get; private set; }

                // these are only needed to show information in console
                public D3BEmu.Common.MPQ.FileFormats.QuestStepObjectiveType ObjectiveType { get { return objective.ObjectiveType; } }
                public int ObjectiveValue { get { return objective.SNOName1.Id; } }

                private D3BEmu.Common.MPQ.FileFormats.QuestStepObjective objective;
                private QuestStep questStep;

                public QuestObjective(D3BEmu.Common.MPQ.FileFormats.QuestStepObjective objective, QuestStep questStep, int id)
                {
                    ID = id;
                    this.objective = objective;
                    this.questStep = questStep;
                }

                /// <summary>
                /// Notifies the objective, that an event occured. The objective checks if that event matches the event it waits for
                /// </summary>
                public void Notify(D3BEmu.Common.MPQ.FileFormats.QuestStepObjectiveType type, int value)
                {
                    if (type != objective.ObjectiveType) return;
                    switch (type)
                    {
                        case D3BEmu.Common.MPQ.FileFormats.QuestStepObjectiveType.EnterWorld:
                        case D3BEmu.Common.MPQ.FileFormats.QuestStepObjectiveType.EnterScene:
                        case D3BEmu.Common.MPQ.FileFormats.QuestStepObjectiveType.InteractWithActor:
                        case D3BEmu.Common.MPQ.FileFormats.QuestStepObjectiveType.KillMonster:
                        case D3BEmu.Common.MPQ.FileFormats.QuestStepObjectiveType.CompleteQuest:
                        case D3BEmu.Common.MPQ.FileFormats.QuestStepObjectiveType.HadConversation:
                        case D3BEmu.Common.MPQ.FileFormats.QuestStepObjectiveType.EnterLevelArea:
                            if (value == objective.SNOName1.Id)
                            {
                                Counter++;
                                questStep.UpdateCounter(this);
                            }
                            break;

                        case D3BEmu.Common.MPQ.FileFormats.QuestStepObjectiveType.EnterTrigger:
                        case D3BEmu.Common.MPQ.FileFormats.QuestStepObjectiveType.EventReceived:
                        case D3BEmu.Common.MPQ.FileFormats.QuestStepObjectiveType.GameFlagSet:
                        case D3BEmu.Common.MPQ.FileFormats.QuestStepObjectiveType.KillGroup:
                        case D3BEmu.Common.MPQ.FileFormats.QuestStepObjectiveType.PlayerFlagSet:
                        case D3BEmu.Common.MPQ.FileFormats.QuestStepObjectiveType.PossessItem:
                        case D3BEmu.Common.MPQ.FileFormats.QuestStepObjectiveType.TimedEventExpired:
                            throw new NotImplementedException();
                    }
                }
            }

            // this is only public for GameCommand / Debug
            public struct ObjectiveSet
            {
                public List<QuestObjective> Objectives;
                public int FollowUpStepID;
            }

            public List<ObjectiveSet> ObjectivesSets = new List<ObjectiveSet>(); // this is only public for GameCommand / Debug
            private List<List<QuestObjective>> bonusObjectives = new List<List<QuestObjective>>();
            private D3BEmu.Common.MPQ.FileFormats.IQuestStep _questStep = null;
            private Quest _quest = null;
            public int QuestStepID { get { return _questStep.ID; } }

            private void UpdateCounter(QuestObjective objective)
            {
                if (_questStep is D3BEmu.Common.MPQ.FileFormats.QuestUnassignedStep == false)
                {
                    foreach (var player in _quest.game.Players.Values)
                        player.InGameClient.SendMessage(new QuestCounterMessage()
                        {
                            snoQuest = _quest.SNOHandle.Id,
                            snoLevelArea = -1,
                            StepID = _questStep.ID,
                            TaskIndex = objective.ID,
                            Counter = objective.Counter,
                            Checked = objective.Done ? 1 : 0,
                        });
                }

                var completedObjectiveList = from objectiveSet in ObjectivesSets
                                             where (from o in objectiveSet.Objectives select o.Done).Aggregate((r, o) => r && o)
                                             select objectiveSet.FollowUpStepID;
                if (completedObjectiveList.Count() > 0)
                    _quest.StepCompleted(completedObjectiveList.First());
            }

            /// <summary>
            /// Debug method, completes a given objective set
            /// </summary>
            /// <param name="index"></param>
            public void CompleteObjectiveSet(int index)
            {
                _quest.StepCompleted(_questStep.StepObjectiveSets[index].FollowUpStepID);
            }

            public QuestStep(D3BEmu.Common.MPQ.FileFormats.IQuestStep assetQuestStep, Quest quest)
            {
                _questStep = assetQuestStep;
                _quest = quest;
                int c = 0;

                foreach (var objectiveSet in assetQuestStep.StepObjectiveSets)
                    ObjectivesSets.Add(new ObjectiveSet()
                    {
                        FollowUpStepID = objectiveSet.FollowUpStepID,
                        Objectives = new List<QuestObjective>(from objective in objectiveSet.StepObjectives select new QuestObjective(objective, this, c++))
                    });
                c = 0;

                if (assetQuestStep is D3BEmu.Common.MPQ.FileFormats.QuestStep)
                {
                    var step = assetQuestStep as D3BEmu.Common.MPQ.FileFormats.QuestStep;

                    if (step.StepBonusObjectiveSets != null)
                        foreach (var objectiveSet in step.StepBonusObjectiveSets)
                            bonusObjectives.Add(new List<QuestObjective>(from objective in objectiveSet.StepBonusObjectives select new QuestObjective(objective, this, c++)));
                }
            }

            public void Notify(D3BEmu.Common.MPQ.FileFormats.QuestStepObjectiveType type, int value)
            {
                foreach (var objectiveSet in ObjectivesSets)
                    foreach (var objective in objectiveSet.Objectives)
                        objective.Notify(type, value);
            }
        }

        public delegate void QuestProgressDelegate(Quest quest);
        public event QuestProgressDelegate OnQuestProgress;
        private D3BEmu.Common.MPQ.FileFormats.Quest asset = null;
        public SNOHandle SNOHandle { get; set; }
        private Game game { get; set; }
        public QuestStep CurrentStep { get; set; }
        private List<int> completedSteps = new List<int>();           // this list has to be saved if quest progress should be saved. It is required to keep track of questranges

        public Quest(Game game, int SNOQuest)
        {
            this.game = game;
            SNOHandle = new SNOHandle(SNOGroup.Quest, SNOQuest);
            asset = SNOHandle.Target as  D3BEmu.Common.MPQ.FileFormats.Quest;
            CurrentStep = new QuestStep(asset.QuestUnassignedStep, this);
        }

        // 
        public bool HasStepCompleted(int stepID)
        {
            return completedSteps.Contains(stepID); // || CurrentStep.ObjectivesSets.Select(x => x.FollowUpStepID).Contains(stepID);
        }

        public void Advance()
        {
            CurrentStep.CompleteObjectiveSet(0);
        }

        public void StepCompleted(int FollowUpStepID)
        {
            foreach (var player in game.Players.Values)
                player.InGameClient.SendMessage(new QuestUpdateMessage()
                {
                    snoQuest = SNOHandle.Id,
                    snoLevelArea = -1,
                    StepID = FollowUpStepID,
                    Field3 = true,
                    Failed = false
                });
            completedSteps.Add(CurrentStep.QuestStepID);
            CurrentStep = (from step in asset.QuestSteps where step.ID == FollowUpStepID select new QuestStep(step, this)).FirstOrDefault();
            if (OnQuestProgress != null)
                OnQuestProgress(this);
        }

        public void Notify(D3BEmu.Common.MPQ.FileFormats.QuestStepObjectiveType type, int value)
        {
            if (CurrentStep != null)
                CurrentStep.Notify(type, value);
        }
    }

}