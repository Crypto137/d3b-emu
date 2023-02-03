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

using System.Collections.Generic;
using System.Linq;
using D3BEmu.Common.Helpers;
using D3BEmu.Common.Helpers.Math;
using D3BEmu.Net.GS.Message;
using D3BEmu.Common;
using D3BEmu.Common.MPQ.FileFormats;
using D3BEmu.Common.MPQ;
using D3BEmu.Core.GS.Common.Types.SNO;
using D3BEmu.Common.Extensions;
using D3BEmu.Common.Logging;

namespace D3BEmu.Core.GS.Items
{
    static class AffixGenerator
    {
        public static readonly Logger Logger = LogManager.CreateLogger();

        private static List<AffixTable> AffixList = new List<AffixTable>();

        static AffixGenerator()
        {
            foreach (var asset in MPQStorage.Data.Assets[SNOGroup.GameBalance].Values)
            {
                GameBalance data = asset.Data as GameBalance;
                if (data != null && data.Type == BalanceType.AffixList)
                {
                    foreach (var affixDef in data.Affixes)
                    {
                        if (affixDef.AffixFamily0 == -1) continue;
                        if (affixDef.Name.Contains("REQ")) continue; // crashes the client // dark0ne
                        if (affixDef.Name.Contains("Sockets")) continue; // crashes the client // dark0ne
                        if (affixDef.Name.Contains("Will")) continue; // not in game // dark0ne

                        AffixList.Add(affixDef);
                    }
                }
            }
        }

        public static void Generate(Item item, int affixesCount)
        {
            if (!Item.IsWeapon(item.ItemType) &&
                !Item.IsArmor(item.ItemType) &&
                !Item.IsOffhand(item.ItemType) &&
                !Item.IsAccessory(item.ItemType))
                return;

            var itemTypes = ItemGroup.HierarchyToHashList(item.ItemType);
            int itemQualityMask = 1 << item.Attributes[GameAttribute.Item_Quality_Level];

            var filteredList = AffixList.Where(a =>
                a.QualityMask.HasFlag((QualityMask)itemQualityMask) &&
                itemTypes.ContainsAtLeastOne(a.ItemGroup) &&
                a.AffixLevel <= item.ItemLevel);

            Dictionary<int, AffixTable> bestDefinitions = new Dictionary<int, AffixTable>();
            foreach (var affix in filteredList)
                bestDefinitions[affix.AffixFamily0] = affix;

            var selectedGroups = bestDefinitions.Values.OrderBy(x => RandomHelper.Next()).Take(affixesCount);

            foreach (var def in selectedGroups)
            {
                if (def != null)
                {
                    //Logger.Debug("Generating affix " + def.Name + " (aLvl:" + def.AffixLevel + ")");
                    item.AffixList.Add(new Affix(def.Hash));
                    foreach (var effect in def.AttributeSpecifier)
                    {
                        if (effect.AttributeId > 0)
                        {
                            float result;
                            if (FormulaScript.Evaluate(effect.Formula.ToArray(), item.RandomGenerator, out result))
                            {
                                //Logger.Debug("Randomized value for attribute " + GameAttribute.Attributes[effect.AttributeId].Name + " is " + result);

                                if (GameAttribute.Attributes[effect.AttributeId] is GameAttributeF)
                                {
                                    var attr = GameAttribute.Attributes[effect.AttributeId] as GameAttributeF;
                                    if (effect.SNOParam != -1)
                                        item.Attributes[attr, effect.SNOParam] += result;
                                    else
                                        item.Attributes[attr] += result;
                                }
                                else if (GameAttribute.Attributes[effect.AttributeId] is GameAttributeI)
                                {
                                    var attr = GameAttribute.Attributes[effect.AttributeId] as GameAttributeI;
                                    if (effect.SNOParam != -1)
                                        item.Attributes[attr, effect.SNOParam] += (int)result;
                                    else
                                        item.Attributes[attr] += (int)result;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void CloneIntoItem(Item source, Item target)
        {
            target.AffixList.Clear();
            foreach (var affix in source.AffixList)
            {
                var newItemAffix = new Affix(affix.AffixGbid);
                target.AffixList.Add(newItemAffix);
            }
            foreach (var affix in target.AffixList)
            {
                var definition = AffixList.Single(def => def.Hash == affix.AffixGbid);
                foreach (var effect in definition.AttributeSpecifier)
                {
                    if (effect.AttributeId <= 0)
                        continue;

                    var attribute = GameAttribute.Attributes[effect.AttributeId];

                    if (attribute.ScriptFunc != null && !attribute.ScriptedAndSettable)
                        continue;

                    if (attribute is GameAttributeF)
                    {
                        var attr = GameAttribute.Attributes[effect.AttributeId] as GameAttributeF;
                        if (effect.SNOParam != -1)
                            target.Attributes[attr, effect.SNOParam] = source.Attributes[attr, effect.SNOParam];
                        else
                            target.Attributes[attr] = source.Attributes[attr];
                    }
                    else if (GameAttribute.Attributes[effect.AttributeId] is GameAttributeI)
                    {
                        var attr = GameAttribute.Attributes[effect.AttributeId] as GameAttributeI;
                        if (effect.SNOParam != -1)
                            target.Attributes[attr, effect.SNOParam] = source.Attributes[attr, effect.SNOParam];
                        else
                            target.Attributes[attr] = source.Attributes[attr];
                    }
                }
            }
        }

    }
}
