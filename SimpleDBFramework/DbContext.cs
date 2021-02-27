using System;
using System.Collections.Generic;
using WebmilioCommons.SQLiteFramework.Mapping;

namespace WebmilioCommons.SQLiteFramework
{
    public abstract class DbContext
    {
        private static readonly Dictionary<Type, DbContextMapping> _map = new Dictionary<Type, DbContextMapping>();
        

        protected DbContext()
        {
            if (!_map.ContainsKey(GetType()))
                MapContext();
        }


        public abstract string GetConnectionString();
        public virtual void OnConfiguring() { }

        private void MapContext()
        {
            _map.Add(GetType(), new DbContextMapping(this));
        }
    }
}
