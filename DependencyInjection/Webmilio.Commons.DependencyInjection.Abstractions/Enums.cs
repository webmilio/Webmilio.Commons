using System;

namespace Webmilio.Commons.DependencyInjection;

[Flags]
public enum MappingBehavior
{
    All = Classes | Interfaces,

    Classes = 0b01,
    Interfaces =  0b10,

    None = 0
}