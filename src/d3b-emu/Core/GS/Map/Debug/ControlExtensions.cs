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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace D3BEmu.Core.GS.Map.Debug
{
    /// <summary>
    /// Provides control extensions.
    /// </summary>
    public static class ControlExtensions
    {
        /// <summary>
        /// Double buffers a given control.
        /// </summary>
        /// <param name="control"></param>
        public static void DoubleBuffer(this Control control)
        {
            // http://stackoverflow.com/questions/76993/how-to-double-buffer-net-controls-on-a-form/77233#77233
            // Taxes: Remote Desktop Connection and painting: http://blogs.msdn.com/oldnewthing/archive/2006/01/03/508694.aspx

            if (SystemInformation.TerminalServerSession) return; // if we're in a terminal server session, just ignore the double-buffer request.
            System.Reflection.PropertyInfo dbProp = typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            dbProp.SetValue(control, true, null);
        }

        /// <summary>
        /// Synchronously invokes supplied delegate on the controls actual thread.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="delegate">The delegate that will be invoked.</param>
        public static void InvokeHandler(this Control control, MethodInvoker @delegate) // Sync. control-invoke extension.
        {
            if (control.InvokeRequired) control.Invoke(@delegate); // if we're not in control's actual thread, switch to it and run the supplied delegate.
            else @delegate(); // if we don't need an invoke, just run the supplied delegate.
        }

        /// <summary>
        /// Asynchronously invokes supplied delegate on the controls actual thread.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="delegate">The delegate that will be invoked.</param>
        public static void AsyncInvokeHandler(this Control control, MethodInvoker @delegate) // Async. control-invoke extension.
        {
            if (control == null) return;
            if (control.InvokeRequired) control.BeginInvoke(@delegate); // if we're not in control's actual thread, switch to it and run the supplied delegate.
            else @delegate(); // if we don't need an invoke, just run the supplied delegate.
        }
    }
}
