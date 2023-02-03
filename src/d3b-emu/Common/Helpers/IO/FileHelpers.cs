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
using System.IO;
using System.Linq;
using System.Reflection;

namespace D3BEmu.Common.Helpers.IO
{
    public static class FileHelpers
    {
        public static string AssemblyRoot
        {
            get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }
        }

        public static List<string> GetFilesByExtensionRecursive(string directory, string fileExtension)
        {
            var files = new List<string>(); // Store results in the file results list.
            var stack = new Stack<string>(); // Store a stack of our directories.

            stack.Push(directory); // Add initial directory.

            while (stack.Count > 0) // Continue while there are directories to process
            {
                var topDir = stack.Pop(); // Get top directory
                var dirInfo = new DirectoryInfo(topDir);

                files.AddRange((from fileInfo in dirInfo.GetFiles()
                                where string.Compare(fileInfo.Extension, fileExtension, System.StringComparison.OrdinalIgnoreCase) == 0
                                select topDir + "/" + fileInfo.Name).ToList());

                foreach (var dir in Directory.GetDirectories(topDir)) // Add all directories at this directory.
                {
                    stack.Push(dir);
                }
            }

            return files.Select(file => file.Replace("\\", "/")).ToList();
        }
    }
}
