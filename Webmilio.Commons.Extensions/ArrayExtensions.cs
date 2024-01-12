using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Webmilio.Commons.Extensions
{
    public static class ArrayExtensions
    {
        public static string Join<T>(this IList<T> items, string separator, Func<T, string> selector) =>
        Join<T>(items, separator, selector, 0, items.Count);

        public static string Join<T>(this IList<T> items, string separator, Func<T, string> selector, int startIndex, int length)
        {
            var values = new string[length];

            for (int i = 0; i < length; i++)
                values[i] = selector(items[i + startIndex]);

            return string.Join(separator, values);
        }

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

        public static async Task DoEnumerableAsync<T>(this IEnumerable<T> source, Func<T, Task> action)
        {
            foreach (var element in source)
                await action(element);
        }


        public static void DoArray(this Array source, Action<object> action) => DoArray(source, (o, _) => action(o));

        public static void DoArray(this Array source, Action<object, int> action)
        {
            for (int i = 0; i < source.Length; i++)
                action(source.GetValue(i), i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Do<T>(this IList<T> source, Action<T> action) => Do(source, (t, i) => action(t));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Do<T>(this IList<T> source, Action<T, int> action) => Do(source, 0, source.Count, action);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Do<T>(this IList<T> source, int startIndex, Action<T> action) => Do(source, startIndex, source.Count, (t, i) => action(t));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Do<T>(this IList<T> source, int startIndex, int length, Action<T, int> action)
        {
            for (int i = startIndex; i < length; i++)
                action(source[i], i);
        }


        public static Task DoAsync<T>(this IList<T> source, Func<T, Task> action) => DoAsync(source, (t, i) => action(t));
        public static Task DoAsync<T>(this IList<T> source, Func<T, int, Task> action) => DoAsync(source, 0, source.Count, action);
        public static Task DoAsync<T>(this IList<T> source, int startIndex, Func<T, Task> action) => DoAsync(source, startIndex, source.Count, (t, i) => action(t));

        public static async Task DoAsync<T>(this IList<T> source, int startIndex, int length, Func<T, int, Task> action)
        {
            for (int i = startIndex; i < length; i++)
            {
                await action(source[i], i);
            }
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

        public static bool All<T>(this IList<T> source, Predicate<T> predicate)
        {
            for (int i = 0; i < source.Count; i++)
            {
                if (!predicate(source[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Any<T>(this IList<T> source, Predicate<T> predicate)
        {
            for (int i = 0; i < source.Count; i++)
            {
                if (predicate(source[i]))
                {
                    return true;
                }
            }

            return false;
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

        public static short Sum<T>(this IList<T> source, Func<T, short> selector)
        {
            short v = 0;

            source.Do(i => v += selector(i));
            return v;
        }

        public static int Sum<T>(this IList<T> source, Func<T, int> selector)
        {
            int v = 0;

            source.Do(i => v += selector(i));
            return v;
        }

        public static float Sum<T>(this IList<T> source, Func<T, float> selector)
        {
            float v = 0;

            source.Do(i => v += selector(i));
            return v;
        }

        public static double Sum<T>(this IList<T> source, Func<T, double> selector)
        {
            double v = 0;

            source.Do(i => v += selector(i));
            return v;
        }

        public static void Move<T>(this IList<T> list, int source, int destination)
        {
            var item = list[source];
            int direction = source > destination ? -1 : 1;

            for (int i = source; i != destination; i += direction)
                list[i] = list[i + direction];

            list[destination] = item;
        }

        public static void Move<T>(this IList<T> list, T element, int destination) =>
            Move(list, list.IndexOf(element), destination);
    }
}