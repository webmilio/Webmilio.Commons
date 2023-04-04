using System;
using System.Collections.Generic;

namespace Webmilio.Commons.Extensions;

public static class DictionaryExtensions
{
    public static TV GetValueOrAdd<TK, TV>(this IDictionary<TK, TV> dictionary, TK key, Func<TK, TV> provider)
    {
        TryGetValueOrAdd(dictionary, key, out var value, provider);
        return value;
    }

    /// <summary></summary>
    /// <typeparam name="TK"></typeparam>
    /// <typeparam name="TV"></typeparam>
    /// <param name="dictionary"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="provider"></param>
    /// <returns><c>true</c> if the value was found; <c>false</c> if the value was acquired from the provider.</returns>
    public static bool TryGetValueOrAdd<TK, TV>(this IDictionary<TK, TV> dictionary, TK key, out TV value,
        Func<TK, TV> provider)
    {
        if (dictionary.TryGetValue(key, out value))
            return true;

        dictionary.Add(key, value = provider(key));
        return false;
    }
}