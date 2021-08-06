using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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


        public static void DoEnumerable<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var element in source)
                action(element);
        }


        public static void DoArray(this Array source, Action<object> action) => DoArray(source, (o, _) => action(o));

        public static void DoArray(this Array source, Action<object, int> action)
        {
            for (int i = 0; i < source.Length; i++)
                action(source.GetValue(i), i);
        }

        public static void Do<T>(this IList<T> source, Action<T> action) => Do(source, (t, i) => action(t));
        public static void Do<T>(this IList<T> source, Action<T, int> action) => Do(source, 0, action);
        public static void Do<T>(this IList<T> source, int startIndex, Action<T> action) => Do(source, startIndex, (t, i) => action(t));

        public static void Do<T>(this IList<T> source, int startIndex, Action<T, int> action)
        {
            for (int i = 0; i < source.Count; i++)
                action(source[i], i);
        }


        public static void Do<T>(this T[,] source, Action<T, int, int> action, int iIndex = 0, int jIndex = 1)
        {
            for (int i = 0; i < source.GetLength(iIndex); i++)
                for (int j = 0; j < source.GetLength(jIndex); j++)
                    action(source[i, j], i, j);
        }


        public static void DoInverted<T>(this IList<T> source, Action<T> action) => DoInverted(source, (t, i) => action(t));

        public static void DoInverted<T>(this IList<T> source, Action<T, int> action)
        {
            for (int i = source.Count - 1; i >= 0; i--)
                action(source[i], i);
        }


        public static void DoEmpty<T>(this List<T> source, Action<T> action)
        {
            for (int i = source.Count - 1; i >= 0; i--)
            {
                action(source[i]);
                source.RemoveAt(i);
            }
        }


        public static void Shuffle<T>(this IList<T> source) => Shuffle(source, new Random());

        // https://stackoverflow.com/questions/273313/randomize-a-listt yoinkers scoobs.
        public static void Shuffle<T>(this IList<T> source, Random rand)
        {
            int n = source.Count;

            while (n > 1)
            {
                n--;

                int k = rand.Next(n + 1);
                T val = source[k];
                
                source[k] = source[n];
                source[n] = val;
            }
        }
    }
}