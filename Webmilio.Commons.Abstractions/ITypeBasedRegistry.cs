using System;

namespace Webmilio.Commons;

public interface ITypeBasedRegistry<T>
{
    public T Get(Type type);

    public bool Has(Type type);
}