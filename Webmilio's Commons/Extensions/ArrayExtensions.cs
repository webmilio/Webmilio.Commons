using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Webmilio.Commons.Extensions
{
    public static class ArrayExtensions
    {
        public static List<string> RemakeEnumerableWithContinuation(this IEnumerable<string> strings, char continuousChar = '"', char seperatorChar = ' ')
        {
            string fullString = string.Join(seperatorChar.ToString(), strings);
            List<string> remake = new List<string>();

            StringBuilder builder = new StringBuilder();
            bool continuous = false;


            void Build()
            {
                remake.Add(builder.ToString());
                builder.Clear();
            }


            for (int i = 0; i < fullString.Length; i++)
            {
                char c = fullString[i];

                if (c == continuousChar)
                {
                    continuous = !continuous;

                    if (!continuous)
                        Build();
                }
                else if (c == seperatorChar)
                {
                    if (!continuous && builder.Length > 0)
                        Build();
                }
                else
                    builder.Append(c);
            }

            if (builder.Length > 0)
                Build();

            return remake;
        }
    }
}