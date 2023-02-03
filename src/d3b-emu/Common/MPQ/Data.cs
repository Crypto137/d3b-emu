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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Gibbed.IO;
using D3BEmu.Core.GS.Common.Types.SNO;
using System.Linq;

namespace D3BEmu.Common.MPQ
{
    public class Data : MPQPatchChain
    {
        public Dictionary<SNOGroup, ConcurrentDictionary<int, Asset>> Assets = new Dictionary<SNOGroup, ConcurrentDictionary<int, Asset>>();
        public readonly Dictionary<SNOGroup, Type> Parsers = new Dictionary<SNOGroup, Type>();
        private readonly List<Task> _tasks = new List<Task>();
        private static readonly SNOGroup[] PatchExceptions = new[] { SNOGroup.TreasureClass, SNOGroup.TimedEvent, SNOGroup.ConversationList };

        public Data()
            : base(7447, new List<string> { "CoreData.mpq", "ClientData.mpq" }, "/base/d3-update-base-(?<version>.*?).mpq")
        { }

        public void Init()
        {
            this.InitCatalog(); // init asset-group dictionaries and parsers.
            this.LoadCatalog(); // process the assets.
        }

        private void InitCatalog()
        {
            foreach (SNOGroup group in Enum.GetValues(typeof(SNOGroup)))
            {
                this.Assets.Add(group, new ConcurrentDictionary<int, Asset>());
            }

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!type.IsSubclassOf(typeof (FileFormat))) continue;
                var attributes = (FileFormatAttribute[])type.GetCustomAttributes(typeof(FileFormatAttribute), true);
                if (attributes.Length == 0) continue;

                Parsers.Add(attributes[0].Group, type);
            }
        }

        private void LoadCatalog()
        {
            var tocFile = this.FileSystem.FindFile("toc.dat");
            if (tocFile == null)
            {
                Logger.Error("Couldn't load CoreData catalog: toc.dat.");
                return;
            }
            
            var stream = tocFile.Open();
            var assetsCount = stream.ReadValueS32();

            var timerStart = DateTime.Now; 

            // read all assets from the catalog first and process them (ie. find the parser if any available).
            while(stream.Position<stream.Length)
            {
                var group = (SNOGroup)stream.ReadValueS32();
                var snoId = stream.ReadValueS32();
                var name = stream.ReadString(128, true);

                var asset = this.ProcessAsset(group, snoId, name); // process the asset.
                this.Assets[group].TryAdd(snoId, asset); // add it to our assets dictionary.
            }

            stream.Close();

            // Run the parsers for assets (that have a parser).

            if (this._tasks.Count > 0) // if we're running in tasked mode, run the parser tasks.
            {
                foreach (var task in this._tasks)
                {
                    task.Start();
                }

                Task.WaitAll(this._tasks.ToArray()); // Wait all tasks to finish.
            }

            GC.Collect(); // force a garbage collection.
            GC.WaitForPendingFinalizers();

            var elapsedTime = DateTime.Now - timerStart;

            Logger.Info("Loaded a total of {0} assets and parsed {1} of them in {2:c}.", assetsCount, this._tasks.Count, elapsedTime);
        }

        private Asset ProcessAsset(SNOGroup group, Int32 snoId, string name)
        {
            var asset = new Asset(group, snoId, name); // create the asset.
            if (!this.Parsers.ContainsKey(asset.Group)) return asset; // if we don't have a proper parser for asset, just give up.

            var parser = this.Parsers[asset.Group]; // get the type the asset's parser.
            var file = this.FileSystem.FindFile(asset.FileName); // get the asset file.

            // if file is in any of the follow groups, try to load the original version - the reason is that assets in those groups got patched to 0 bytes.
            if (PatchExceptions.Contains(asset.Group))
            {
                foreach (CrystalMpq.MpqArchive archive in this.FileSystem.Archives.Reverse()) //search mpqs starting from base
                {
                    file = archive.FindFile(asset.FileName);
                    if (file != null)
                        break;
                }
            }

            if (file == null || file.Size < 10) return asset; // if it's empty, give up again.

            if (Storage.Config.Instance.EnableTasks)
                this._tasks.Add(new Task(() => asset.RunParser(parser, file))); // add it to our task list, so we can parse them concurrently.        
            else
                asset.RunParser(parser, file); // run the parsers sequentally.
    
            return asset;
        }
    }
}
