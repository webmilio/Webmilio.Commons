using Webmilio.Commons.DependencyInjection;
using Webmilio.Commons.Helpers;

namespace Webmilio.Commons.Mapping
{
    public class Mapper
    {
        public static T Map<T>(object origin)
        {
            T target = TypeHelpers.Instantiate<T>();

            return Map(origin, target);
        }

        public static T Map<T>(object origin, ServiceProvider services)
        {
            var target = services.Make<T>();

            return Map(origin, target);
        }

        private static T Map<T>(object origin, T target)
        {
            var originProperties = origin.GetType().GetProperties();
            var targetProperties = typeof(T).GetProperties();

            foreach (var tp in targetProperties)
            {
                foreach (var op in originProperties)
                {
                    if (tp.Name != op.Name || tp.PropertyType != op.PropertyType)
                        continue;

                    var value = op.GetValue(origin);

                    if (value != default)
                        tp.SetValue(target, value);
                }
            }

            return target;
        }
    }
}