using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class CommandSourceSample
{
    public static async Task Interceptors_get_the_source_of_the_command()
    {
        Console.WriteLine($">>>> Sample: {nameof(Interceptors_get_the_source_of_the_command)}");
        Console.WriteLine();

        using var context = new CustomersContext();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        context.Add(
            new Customer
            {
                Name = "Sam Vimes"
            });

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var customers = await context.Customers.ToListAsync();

        Console.WriteLine();
    }

    #region Interceptor
    public class CommandSourceInterceptor : DbCommandInterceptor
    {
        public override InterceptionResult<DbDataReader> ReaderExecuting(
            DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            if (eventData.CommandSource == CommandSource.SaveChanges)
            {
                Console.WriteLine($"Saving changes for {eventData.Context!.GetType().Name}:");
                Console.WriteLine();
                Console.WriteLine(command.CommandText);
            }

            return result;
        }
    }
    #endregion

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CustomersContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .AddInterceptors(new CommandSourceInterceptor())
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCoreSample;ConnectRetryCount=0");
        }
    }
}
