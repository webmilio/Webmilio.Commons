using System;

namespace Webmilio.Commons.Extensions
{
    public static class EnumHelpers
    {
        private static readonly Random _random = new();


        public static T Random<T>() where T : Enum => Random<T>(_random);

        public static T Random<T>(Random random) where T : Enum
        {
            var values = GetValues<T>();

            return values[random.Next(0, values.Length)];
        }

        
        public static T[] GetValues<T>() where T : Enum
        {
            var values = Enum.GetValues(typeof(T));
            var genericValues = new T[values.Length];

            values.DoArray((o, i) => genericValues[i] = (T) o);

            return genericValues;
        }

        
        public static string[] GetNames<T>() where T : Enum
        {
            return Enum.GetNames(typeof(T));
        }
    }
}