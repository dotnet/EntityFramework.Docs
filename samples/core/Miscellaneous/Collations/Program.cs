using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFCollations;

public class Program
{
    public static async Task Main(string[] args)
    {
        using (var db = new CustomerContext())
        {
            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();
        }

        using (var context = new CustomerContext())
        {
            #region SimpleQueryCollation
            var customers = await context.Customers
                .Where(c => EF.Functions.Collate(c.Name, "SQL_Latin1_General_CP1_CS_AS") == "John")
                .ToListAsync();
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region DatabaseCollation
        modelBuilder.UseCollation("SQL_Latin1_General_CP1_CS_AS");
        #endregion

        #region ColumnCollation
        modelBuilder.Entity<Customer>().Property(c => c.Name)
            .UseCollation("SQL_Latin1_General_CP1_CI_AS");
        #endregion
    }
}

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
}
