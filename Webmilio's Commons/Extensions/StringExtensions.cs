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


            if (builder.Length > 0)
                AddBuilder();


            return built;
        }
    }
}