using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SqlServer.ValueGeneration;

public class ExplicitIdentityValuesContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(
            @"Server=(localdb)\mssqllocaldb;Database=EFSaving.Basics;Trusted_Connection=True");
}

public class ExplicitIdentityValues
{
    public static async Task Run()
    {
        using (var context = new ExplicitIdentityValuesContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        #region ExplicitIdentityValues
        using (var context = new ExplicitIdentityValuesContext())
        {
            context.Blogs.Add(new Blog { BlogId = 100, Url = "http://blog1.somesite.com" });
            context.Blogs.Add(new Blog { BlogId = 101, Url = "http://blog2.somesite.com" });

            await context.Database.OpenConnectionAsync();
            try
            {
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Blogs ON");
                await context.SaveChangesAsync();
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Blogs OFF");
            }
            finally
            {
                await context.Database.CloseConnectionAsync();
            }
        }
        #endregion
    }
}
