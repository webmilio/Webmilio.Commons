using System;
using WebmilioCommons.SQLiteFramework;

namespace SQLiteFramework.Sandbox
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var context = new TestDBContext();

            Console.ReadLine();
        }
    }

    public class TestDBContext : DbContext
    {
        public TestDBContext()
        {
        }


        public override string GetConnectionString()
        {
            return null;
        }


        public DbSet<TableX> Xs { get; set; }
    }

    public class TableX
    {
        public int Id { get; set; }
    }
}
