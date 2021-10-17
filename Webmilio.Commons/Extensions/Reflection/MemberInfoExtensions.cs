﻿using System;
using System.Reflection;

namespace Webmilio.Commons.Extensions.Reflection
{
    public static class MemberInfoExtensions
    {
        public static bool TryGetCustomAttribute<T>(this MemberInfo member, out T attribute) where T : Attribute
        {
            attribute = member.GetCustomAttribute<T>();
            return attribute != null;
        }
    }
}