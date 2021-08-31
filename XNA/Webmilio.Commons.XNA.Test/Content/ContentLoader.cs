using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Webmilio.Commons;
using Webmilio.Commons.Extensions;

namespace Webmilio.Electronics.Builder.Content
{
    public abstract class ContentLoader
    {
        public abstract void Load(Game builder);

        public abstract string Path { get; }
    }

    public abstract class ContentLoader<T> : ContentLoader
    {
        public override void Load(Game builder)
        {
            GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(T) && p.GetCustomAttribute<IgnoreAttribute>() == default)
                .DoEnumerable(p => LoadItem(builder, p));
        }

        protected virtual void LoadItem(Game game, PropertyInfo property)
        {
            property.SetValue(this, game.Content.Load<T>($"{Path}/{property.Name}"));
        }
    }
}