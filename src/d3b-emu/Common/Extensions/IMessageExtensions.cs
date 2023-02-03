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
using System.Text;
using Google.ProtocolBuffers;
using Google.ProtocolBuffers.Descriptors;

namespace D3BEmu.Common.Extensions
{
    static class IMessageExtensions
    {

        static void AppendLevel(StringBuilder result, int level)
        {
            for (int i = 0; i < level; i++) result.Append(' ');
        }

        static void AppendLine(StringBuilder result, int level, string str) { for (int i = 0; i < level; i++) result.Append(' '); result.AppendLine(str); }
        static char ToHexChar(byte b)
        {
            return (b >= 0x20 && b < 0x80) ? (char)b : '.';
        }

        static void AppendHexdump(StringBuilder b, int level, byte[] buffer)
        {
            b.AppendLine();
            AppendLine(b, level++, "[0x" + buffer.Length.ToString("X8") + "] {");

            int length = Math.Min(buffer.Length, 0xFFFF);
            for (int i = 0; i < length; i += 16)
            {

                AppendLevel(b, level);

                b.AppendFormat("{0:X4}  ", i);
                for (int n = 0; n < 8; n++)
                {
                    int o = i + n;
                    if (o < length)
                        b.AppendFormat("{0:X2} ", buffer[o]);
                    else
                        b.Append("   ");
                }
                b.Append(" ");
                for (int n = 0; n < 8; n++)
                {
                    int o = i + n + 8;
                    if (o < length)
                        b.AppendFormat("{0:X2} ", buffer[o]);
                    else
                        b.Append("   ");
                }
                b.Append(" ");

                for (int n = 0; n < 8; n++)
                {
                    int o = i + n;
                    if (o < length)
                        b.AppendFormat("{0}", ToHexChar(buffer[o]));
                    else
                        b.Append(" ");
                }
                b.Append(" ");
                for (int n = 0; n < 8; n++)
                {
                    int o = i + n + 8;
                    if (o < length)
                        b.AppendFormat("{0}", ToHexChar(buffer[o]));
                    else
                        b.Append(" ");
                }
                b.AppendLine();
            }
            AppendLine(b, --level, "}");
        }

        static void AppendFieldValue(StringBuilder result, int level, FieldType type, object value)
        {
            switch (type)
            {
                case FieldType.Bool:
                    result.AppendLine((bool)value ? "true" : "false");
                    break;
                case FieldType.Bytes:
                    AppendHexdump(result, level, ((ByteString)value).ToByteArray());
                    break;
                case FieldType.Double:
                    result.AppendLine(((double)value).ToString("G"));
                    break;
                case FieldType.Float:
                    result.AppendLine(((float)value).ToString("G"));
                    break;
                case FieldType.Int32:
                case FieldType.SFixed32:
                case FieldType.SInt32:
                    result.AppendLine(value.ToString());
                    break;
                case FieldType.Int64:
                case FieldType.SFixed64:
                case FieldType.SInt64:
                    result.AppendLine("0x" + ((long)value).ToString("X16") + "l");
                    break;
                case FieldType.Fixed32:
                case FieldType.UInt32:
                    result.AppendLine("0x" + ((uint)value).ToString("X8") + "u");
                    break;
                case FieldType.Fixed64:
                case FieldType.UInt64:
                    result.AppendLine("0x" + ((ulong)value).ToString("X16") + "ul");
                    break;
                case FieldType.Message:
                    result.AppendLine();
                    AppendMessage(result, level, (IMessage)value);
                    break;
                case FieldType.Enum:
                    {
                        EnumValueDescriptor e = (EnumValueDescriptor)value;

                        result.AppendLine(e.Name);
                    }
                    break;
                case FieldType.String:
                    result.AppendLine(value.ToString());
                    break;
                case FieldType.Group:
                default:
                    throw new Exception("Unhandled FieldType");
            }
        }
        static void AppendField(StringBuilder result, int level, FieldDescriptor fieldDesc, object value)
        {
            if (value == null)
                return;
            if (fieldDesc.IsRepeated)
            {
                var e = ((System.Collections.IEnumerable)value).GetEnumerator();
                while (e.MoveNext())
                {
                    AppendLevel(result, level);
                    result.Append(fieldDesc.Name);
                    result.Append(": ");
                    AppendFieldValue(result, level, fieldDesc.FieldType, e.Current);
                }
            }
            else
            {
                AppendLevel(result, level);
                result.Append(fieldDesc.Name);
                result.Append(": ");
                AppendFieldValue(result, level, fieldDesc.FieldType, value);
            }
        }

        static void AppendMessage(StringBuilder result, int level, IMessage msg)
        {
            AppendLine(result, level++, "{");
            var fields = msg.AllFields;
            foreach (var pair in fields)
                AppendField(result, level, pair.Key, pair.Value);
            AppendLine(result, --level, "}");
        }

        public static string AsText(this IMessage msg)
        {
            var msgDesc = msg.DescriptorForType;
            StringBuilder result = new StringBuilder();
            result.AppendLine(msgDesc.FullName);
            AppendMessage(result, 0, msg);
            return result.ToString();
        }
    }
}
