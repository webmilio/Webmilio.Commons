using System.Collections.Generic;
using System.Linq;
using Webmilio.Commons.Extensions;

namespace WebmilioCommons.SQLiteFramework.Mapping
{
    public class DbContextMapping
    {
        public DbContextMapping(DbContext context)
        {
            TableNames = new List<TableMapping>();
            MapTables(context);
        }


        private void MapTables(DbContext context)
        {
            context.GetType().GetProperties()
                .Where(p => p.PropertyType == typeof(DbSet<>))
                .Do(p => TableNames.Add(new TableMapping(p)));
        }
        

        public List<TableMapping> TableNames { get; }
    }
}