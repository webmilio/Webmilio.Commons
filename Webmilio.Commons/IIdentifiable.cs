﻿namespace Webmilio.Commons;

public interface IIdentifiable<out T>
{
    public T Identifier { get; }
}