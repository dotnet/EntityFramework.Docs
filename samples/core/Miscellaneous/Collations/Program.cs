using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EFCollations
{
    public class Program
    {
        static void Main(string[] args)
        {
            using (var db = new CustomerContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }

            using (var context = new CustomerContext())
            {
                #region SimpleQueryCollation
                var customers = context.Customers
                    .Where(c => EF.Functions.Collate(c.Name, "SQL_Latin1_General_CP1_CS_AS") == "John")
                    .ToList();
                #endregion
            }
        }
    }

    public class CustomerContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCollations;Trusted_Connection=True;ConnectRetryCount=0");
        }

        #region OnModelCreating
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("SQL_Latin1_General_CP1_CS_AS");

            modelBuilder.Entity<Customer>().Property(c => c.Name)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
        }
        #endregion
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
