using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Webmilio.Commons.ComponentModel.DataAnnotations;
using Webmilio.Commons.Extensions;

namespace Webmilio.Commons.EntityFrameworkCore
{
    public class DbContext<T> : DbContext where T : DbContext<T>
    {
        public DbContext(DbContextOptions<T> options) : base(options)
        {
        }


        public Task Initialize()
        {
            return Database.EnsureCreatedAsync();
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            ApplyAnnotationAttributes(builder);

            ModModelCreating(builder);
        }

        protected virtual void ModModelCreating(ModelBuilder builder)
        {
        }

        protected virtual void ApplyAnnotationAttributes(ModelBuilder builder)
        {
            builder.Model.GetEntityTypes().DoEnumerable(delegate(IMutableEntityType et)
            {
                List<AnnotationAttribute> entityAttributes = new();
                Dictionary<string, object> entityMetadata = new();

                et.GetProperties().DoEnumerable(delegate(IMutableProperty p)
                {
                    if (p.PropertyInfo == default)
                        return;

                    List<AnnotationAttribute> propertyAttributes = new();

                    void PropertyLoop(AnnotationAttribute aa)
                    {
                        entityAttributes.Add(aa);
                        propertyAttributes.Add(aa);

                        aa.PropertyLoop(p.PropertyInfo, entityMetadata);

                        var fields = aa.PropertyMetadatas;

                        if (fields != default)
                        {
                            if (fields.Precision.HasValue) p.SetPrecision(fields.Precision);
                            if (fields.Scale.HasValue) p.SetScale(fields.Scale);
                        }
                    }

                    p.PropertyInfo.GetCustomAttributes<AnnotationAttribute>()
                        .Where(aa => aa.CheckTypeCompatibility(p.ClrType))
                        .DoEnumerable(PropertyLoop);

                    propertyAttributes.Do(pa => pa.PostPropertyLoop(p.PropertyInfo, entityMetadata));
                });

                entityAttributes.Do(ea => ea.PostEntityLoop(et.ClrType, entityMetadata));
                entityAttributes.Do(delegate(AnnotationAttribute attribute)
                {
                    if (attribute.EntityMetadatas == default)
                    {
                        return;
                    }

                    DoKeyList(attribute.EntityMetadatas.Keys, et, (t, p) => t.SetPrimaryKey(p));
                    DoKeyList(attribute.EntityMetadatas.AlternateKeys, et, (t, p) => 
                        p.Do(x => t.AddKey(x)));
                });
            });
        }

        private void DoKeyList(List<string> keys, IMutableEntityType et, KeyAction action)
        {
            if (keys == default || keys.Count == 0)
                return;

            List<IMutableProperty> properties = new();
            keys.Do(k => properties.Add(et.FindProperty(k)));

            action(et, properties);
        }

        private delegate void KeyAction(IMutableEntityType entityType, List<IMutableProperty> properties);
    }
}