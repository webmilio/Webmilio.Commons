using System;

namespace Webmilio.Commons.Helpers
{
    public static class TypeHelpers
    {
        public static T Instantiate<T>(params object[] args)
        {
            return (T) Activator.CreateInstance(typeof(T), args);
        }
    }
}