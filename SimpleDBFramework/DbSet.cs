using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WebmilioCommons.SQLiteFramework
{
    public class DbSet<T> : Collection<T>
    {
        private readonly DbContext _context;


        public DbSet(DbContext context)
        {
            _context = context;
        }



    }
}