using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WindowsHostDistributer
{
    public static class Extensions
    {
        #region MemoryStream Read and Write
        public static unsafe short ReadInt16(this MemoryStream stream)
        {
            return (short)(
                stream.GetBuffer()[stream.Position++] |
                stream.GetBuffer()[stream.Position++] << 8);
        }

        public static unsafe int ReadInt32(this MemoryStream stream)
        {
            var buf = stream.GetBuffer();
            return
                buf[stream.Position++] |
                buf[stream.Position++] << 8 |
                buf[stream.Position++] << 16 |
                buf[stream.Position++] << 24;
        }

        public static unsafe long ReadInt64(this MemoryStream stream)
        {
            var buf = stream.GetBuffer();
            return
                buf[stream.Position++] |
                buf[stream.Position++] << 8 |
                buf[stream.Position++] << 16 |
                buf[stream.Position++] << 24 |
                buf[stream.Position++] << 32 |
                buf[stream.Position++] << 40 |
                buf[stream.Position++] << 48 |
                buf[stream.Position++] << 56;
        }

        public static byte[] ReadBytes(this MemoryStream stream, int length)
        {
            var bytes = new byte[length];
            stream.Read(bytes, 0, length);
            return bytes;
        }

        public static unsafe void Write(this MemoryStream stream, short value)
        {
            byte* ptr = (byte*)&value;

            for (int i = 0; i < 2; ++i)
                stream.WriteByte(*ptr++);
        }

        public static unsafe void Write(this MemoryStream stream, int value)
        {
            byte* ptr = (byte*)&value;

            for (int i = 0; i < 4; ++i)
                stream.WriteByte(*ptr++);
        }

        public static unsafe void Write(this MemoryStream stream, long value)
        {
            byte* ptr = (byte*)&value;

            for (int i = 0; i < 8; ++i)
                stream.WriteByte(*ptr++);
        }

        public static void Write(this MemoryStream stream, byte[] data)
        {
            stream.Write(data, 0, data.Length);
        }

        public static string ReadPrefixString(this MemoryStream stream)
        {
            return Encoding.GetEncoding(1252).GetString(stream.ReadBytes(stream.ReadInt32()));
        }

        public static void WritePrefixString(this MemoryStream stream, string text)
        {
            stream.Write(text.Length);
            stream.Write(Encoding.GetEncoding(1252).GetBytes(text));
        }

        public static void Delete(this MemoryStream stream, int length)
        {
            if (length == stream.Length)
            {
                stream.SetLength(0);
                return;
            }

            var buffer = stream.GetBuffer();

            Buffer.BlockCopy(buffer, length, buffer, 0, (int)(stream.Length - length));
            stream.SetLength(stream.Length - length);
        }

        public static int Find(this MemoryStream stream, byte value)
        {
            var data = stream.GetBuffer();
            for (int i = 0; i < stream.Length; ++i)
            {
                if (data[i] == value)
                    return i;
            }

            return -1;
        }

        public static int Find(this MemoryStream stream, char value)
        {
            return stream.Find((byte)value);
        }

        public static byte[] Extract(this MemoryStream stream, int offset, int length)
        {
            if (offset < 0 ||
                offset + length > stream.Length)
                throw new ArgumentOutOfRangeException("The selected region of bytes are out of range in the stream.");

            var data = stream.GetBuffer();
            var result = new byte[length];

            // copy data to be extract
            Buffer.BlockCopy(data, offset, result, 0, length);

            // remove the data from the stream
            Buffer.BlockCopy(data, offset + length, data, offset, (int)stream.Length - (offset + length));
            stream.SetLength(stream.Length - length);

            return result;
        }
        #endregion

        #region .NET 4.5 stuff
        public static T GetCustomAttribute<T>(this MemberInfo member) where T : Attribute
        {
            return (T)member.GetCustomAttributes(typeof(T), false).FirstOrDefault();
        }
        #endregion

        public static string Escape(this string str, EscapeLanguage language)
        {
            var sb = new StringBuilder(str.Length);

            if (language == EscapeLanguage.Xml)
            {
                foreach (char cb in str)
                {
                    switch (cb)
                    {
                        case '"':
                            sb.Append("&quot;");
                            break;
                        case '\'':
                            sb.Append("&apos;");
                            break;
                        case '<':
                            sb.Append("&lt;");
                            break;
                        case '>':
                            sb.Append("&gt;");
                            break;
                        case '&':
                            sb.Append("&amp;");
                            break;
                        case '\r':
                            sb.Append("&#13;");
                            break;
                        case '\n':
                            sb.Append("&#10;");
                            break;
                        case '\t':
                            sb.Append("&#9;");
                            break;
                        case '|':
                            sb.Append("&#124;");
                            break;
                        case '/':
                            sb.Append("&#x2f;");
                            break;
                        default:
                            sb.Append(cb);
                            break;
                    }
                }
            }

            return sb.ToString();
        }

        public static string Unescape(this string str, EscapeLanguage language)
        {
            var sb = new StringBuilder(str.Length);

            if (language == EscapeLanguage.Xml)
            {
                for (int i = 0; i < str.Length;)
                {
                    if (str[i] == '&')
                    {
                        if (++i >= str.Length)
                            throw new Exception("Invalid XmlString");

                        // find ;
                        int endIndex = str.IndexOf(';', i);
                        if (endIndex == -1 ||
                            endIndex - i == 0) // same as &;
                            throw new Exception("Invalid XmlString");

                        var stringAttribute = str.Substring(i, endIndex - i);

                        if (stringAttribute[0] == '#')
                        {
                            if (stringAttribute.Length < 2 || // &#1;
                                (char.ToLower(stringAttribute[1]) == 'x' && stringAttribute.Length < 3)) // &#x1;
                                throw new Exception("Invalid XmlString");

                            if (char.ToLower(stringAttribute[1]) == 'x')
                            {
                                var hex = stringAttribute.Substring(2).PadLeft(4, '0');
                                var val = (char)short.Parse(hex, NumberStyles.HexNumber);
                                sb.Append(val);
                            }
                            else
                            {
                                // number
                                var val = (char)short.Parse(stringAttribute.Substring(1));
                                sb.Append(val);
                            }
                        }
                        else
                        {
                            switch (stringAttribute.ToLower())
                            {
                                case "quot":
                                    sb.Append('"');
                                    break;
                                case "apos":
                                    sb.Append('\'');
                                    break;
                                case "lt":
                                    sb.Append('<');
                                    break;
                                case "gt":
                                    sb.Append('>');
                                    break;
                                case "amp":
                                    sb.Append('&');
                                    break;
                            }
                        }

                        // parse this attribute

                        i = endIndex + 1;
                        continue;
                    }

                    sb.Append(str[i++]);
                }
            }

            return sb.ToString();
        }
    }

    public enum EscapeLanguage
    {
        Xml
    }
}
