using System.Linq;
using Webmilio.Commons.Extensions;
using Webmilio.Commons.Extensions.Reflection;

namespace Webmilio.Commons.Helpers;

public static class ObjectHelpers
{
    public static void ClearProperties(object obj)
    {
        obj.GetType().GetProperties()
            .Where(p => p.SetMethod != default)
            .DoEnumerable(p =>
        {
            if (p.PropertyType.IsValueType)
            {
                p.SetValue(obj, p.PropertyType.Create());
            }
            else
            {
                p.SetValue(obj, null);
            }
        });
    }
}
