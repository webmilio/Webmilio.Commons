using System;
using System.Collections.Generic;
using System.Text;

namespace Webmilio.Commons.Extensions
{
    public static class StringExtensions
    {
        public static int UpperCount(this string str)
        {
            int total = 0;

            for (int i = 0; i < str.Length; i++)
                if (char.IsUpper(str[i]))
                    total++;

            return total;
        }

        public static int LowerCount(this string str)
        {
            int total = 0;

            for (int i = 0; i < str.Length; i++)
                if (char.IsLower(str[i]))
                    total++;

            return total;
        }


        public static List<string> SplitEveryCapital(this string str)
        {
            var built = new List<string>(UpperCount(str));
            var builder = new StringBuilder();


            void AddBuilder()
            {
                built.Add(builder.ToString());
                builder.Clear();
            }


            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];

                if (c == ' ')
                {
                    if (builder.Length > 0)
                        AddBuilder();
                }
                else if (char.IsUpper(c))
                {
                    if (builder.Length > 0)
                    {
                        AddBuilder();
                        builder.Append(c);
                    }
                }
                else
                    builder.Append(c);
            }


            if (builder.Length > 0)
                AddBuilder();


            return built;
        }

        public static List<string> SplitDelimiter(this string str, char delimiter = '"')
        {
            List<string> built = new();
            StringBuilder builder = new();
            bool del = false;


            void AddBuilder()
            {
                if (builder.Length == 0)
                    return;

                built.Add(builder.ToString());
                builder.Clear();
            }


            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];

                if (c == delimiter)
                {
                    if (del)
                        AddBuilder();

                    del = !del;
                }
                else
                {
                    if (c == ' ') 
                    {
                        if (del)
                            builder.Append(c);
                        else
                            AddBuilder();
                    }
                    else
                    {
                        builder.Append(c);
                    }
                }
            }


            if (builder.Length > 0)
                AddBuilder();


            return built;
        }


        public static string EnsureLength(this string str, int length, char fillWith = ' ')
        {
            if (str.Length > length)
                throw new Exception("The provided string must be smaller or equal to the provided length.");

            if (str.Length == length)
                return str;

            return str.PadRight(length, fillWith);
        }

        public static bool EdgesWith(this string str, char c)
        {
#if NETSTANDARD2_1
            return str.StartsWith(c) && str.EndsWith(c);
#else
            return str[0] == c && str[str.Length - 1] == c;
#endif
        }


        public static StringBuilder SpaceFormatString(this StringBuilder sb, string format, params object[] args)
        {
            if (sb.Length > 0)
                sb.Append(' ');

            sb.AppendFormat(format, args);
            return sb;
        }
    }
}