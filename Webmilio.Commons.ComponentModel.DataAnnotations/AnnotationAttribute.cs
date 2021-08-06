using System;
using System.Collections.Generic;
using System.Reflection;

namespace Webmilio.Commons.ComponentModel.DataAnnotations
{
    public abstract class AnnotationAttribute : Attribute
    {
        public virtual bool CheckTypeCompatibility(Type clrType) => true;


        public virtual void PropertyLoop(PropertyInfo property, Dictionary<string, object> entityMetadata) { }

        public virtual void PostPropertyLoop(PropertyInfo property, Dictionary<string, object> entityMetadata) { }
        public virtual void PostEntityLoop(Type type, Dictionary<string, object> entityMetadata) { }

        public PropertyMetadatas PropertyMetadatas { get; protected set; }
        public EntityMetadatas EntityMetadatas { get; protected set; }
    }
}