using System;
using System.Collections.Generic;

namespace Webmilio.Commons;

public class TypeBasedRegistry<T> : ITypeBasedRegistry<T>
{
    protected readonly Dictionary<Type, T> serializers = new();

    /// <summary>Replaces a serializer for another.</summary>
    /// <param name="type">The property type.</param>
    /// <param name="serializer">The serializer (reader/writer) for the type.</param>
    public virtual T Replace(Type type, T serializer)
    {
        serializers.TryGetValue(type, out var old);
        serializers.Remove(type);

        serializers.Add(type, serializer);
        return old;
    }

    /// <summary>Adds many serializers for the given types.</summary>
    /// <param name="type">The type.</param>
    /// <param name="serializer">The serializer (reader/writer) for the type.</param>
    public void Add(params (Type type, T serializer)[] serializers)
    {
        for (int i = 0; i < serializers.Length; i++)
        {
            Add(serializers[i].type, serializers[i].serializer);
        }
    }

    /// <summary>Add a serializer for the given type.</summary>
    /// <typeparam name="K">The type.</typeparam>
    /// <param name="serializer">The serializer (reader/writer) for the type.</param>
    public void Add<K>(T serializer) => Add(typeof(K), serializer);

    /// <summary>Add a serializer for the given type.</summary>
    /// <param name="type">The type.</param>
    /// <param name="serializer">The serializer (reader/writer) for the type.</param>
    public virtual void Add(Type type, T serializer)
    {
        serializers.Add(type, serializer);
    }

    /// <summary>Check if there is a serializer defined for a type.</summary>
    /// <param name="type">The type.</param>
    /// <returns><c>true</c> if there is a serializer for the type; otherwise false.</returns>
    public virtual bool Has(Type type)
    {
        return serializers.ContainsKey(type);
    }

    /// <summary>Check if there is a serializer defined for a type.</summary>
    /// <typeparam name="K">The type.</typeparam>
    /// <returns><c>true</c> if there is a serializer for the type; otherwise false.</returns>
    public bool Has<K>() => Has(typeof(K));

    /// <summary>Fetch a serializer.</summary>
    /// <param name="type">The type.</param>
    /// <returns>The <typeparamref name="T"/> if found; otherwise <c>null</c>.</returns>
    public virtual T Get(Type type)
    {
        return serializers[type];
    }

    /// <summary>Tries to get a serializer with the associated type.</summary>
    /// <returns><c>true</c> if a serializer was found; otherwise <c>false</c>.</returns>
    public bool TryGet(Type type, out T serializer)
    {
        serializer = default;

        if (!Has(type))
        {
            return false;
        }

        serializer = Get(type);
        return true;
    }

    /// <summary>Fetch a serializer.</summary>
    /// <typeparam name="K">The type.</typeparam>
    /// <returns>The <typeparamref name="T"/> if found; otherwise <c>null</c>.</returns>
    public T Get<K>() => Get(typeof(K));
}
