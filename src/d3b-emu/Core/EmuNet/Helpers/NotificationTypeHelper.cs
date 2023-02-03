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

namespace D3BEmu.Core.EmuNet.Helpers
{
    public static class NotificationTypeHelper
    {
        /// <summary>
        /// Returns the NotificationType for the given notification.
        /// </summary>
        /// <param name="notification">The notification</param>
        /// <returns><see cref="NotificationType"/></returns>
        public static NotificationType GetNotificationType(this bnet.protocol.notification.Notification notification)
        {
            switch (notification.Type)
            {
                case "WHISPER":
                    return NotificationType.Whisper;
            }
            return NotificationType.Unknown;
        }

        /// <summary>
        /// Notification types
        /// </summary>
        public enum NotificationType
        {
            Unknown,
            Whisper
        }
    }
}
