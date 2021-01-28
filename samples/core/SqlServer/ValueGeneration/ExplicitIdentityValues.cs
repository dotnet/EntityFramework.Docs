using Microsoft.EntityFrameworkCore;

namespace SqlServer.ValueGeneration
{
    public class ExplicitIdentityValuesContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=EFSaving.Basics;Trusted_Connection=True;ConnectRetryCount=0");
    }

    public class ExplicitIdentityValues
    {
        public static void Run()
        {
            using (var context = new ExplicitIdentityValuesContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            #region ExplicitIdentityValues
            using (var context = new ExplicitIdentityValuesContext())
            {
                context.Blogs.Add(new Blog { BlogId = 100, Url = "http://blog1.somesite.com" });
                context.Blogs.Add(new Blog { BlogId = 101, Url = "http://blog2.somesite.com" });

                context.Database.OpenConnection();
                try
                {
                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Employees ON");
                    context.SaveChanges();
                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Employees OFF");
                }
                finally
                {
                    context.Database.CloseConnection();
                }
            }
            #endregion
        }
    }
}
