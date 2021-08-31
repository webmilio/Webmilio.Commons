using System.Collections.Generic;
using System.Reflection;

namespace Webmilio.Commons.ComponentModel.DataAnnotations
{
    /// <summary>Denotes one or more properties that uniquely identify an entity.</summary>
    public class KeyFragmentAttribute : AnnotationAttribute
    {
        private const string KeyName = nameof(KeyFragmentAttribute);


        public override void PropertyLoop(PropertyInfo property, Dictionary<string, object> entityMetadata)
        {
            List<string> fragments;

            if (entityMetadata.TryGetValue(KeyName, out var oFragments))
                fragments = (List<string>) oFragments;
            else
            {
                fragments = new();
                entityMetadata.Add(KeyName, fragments);
            }

            fragments.Add(property.Name);
        }

        public override void PostPropertyLoop(PropertyInfo property, Dictionary<string, object> entityMetadata)
        {
            if (!entityMetadata.TryGetValue(KeyName, out var oFragments))
                return;

            var fragments = (List<string>) oFragments;
            EntityMetadatas = new EntityMetadatas
            {
                Keys = fragments
            };
        }
    }
}