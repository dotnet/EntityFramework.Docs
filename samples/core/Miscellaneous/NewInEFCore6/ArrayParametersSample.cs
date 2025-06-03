using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class ArrayParametersSample
{
    public static async Task Array_parameters_are_logged_in_readable_form()
    {
        Console.WriteLine($">>>> Sample: {nameof(Array_parameters_are_logged_in_readable_form)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();

        using var context = new SomeDbContext();

        try
        {
            await context.Database.ExecuteSqlRawAsync(
                "SELECT * FROM Blogs WHERE Data = {0}",
                new SqlParameter
                {
                    DbType = DbType.String, Value = new[] { "First", "Second", "Third", "Fourth", "Fifth", "Sixth", "Seventh" }
                });
        }
        catch
        {
            // Exception is expected as array parameters are not valid on SQL Server.
            // However, log output shows the parameter logging.
            //
            // dbug: 8/2/2021 11:34:16.880 RelationalEventId.CommandExecuting[20100] (Microsoft.EntityFrameworkCore.Database.Command)
            // Executing DbCommand [Parameters=[@p0={ 'First', 'Second', 'Third', 'Fourth', 'Fifth', ... } (Nullable = false) (DbType = String)], CommandType='Text', CommandTimeout='30']
            // SELECT * FROM Blogs WHERE Data = @p0
        }

        Console.WriteLine();
    }

    public static class Helpers
    {
        public static async Task RecreateCleanDatabase()
        {
            using var context = new IsNullOrWhitespaceSample.BooksContext(quiet: true);

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }
    }

    public class SomeDbContext : DbContext
    {
        private readonly bool _quiet;

        public SomeDbContext(bool quiet = false)
        {
            _quiet = quiet;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCoreSample;ConnectRetryCount=0");

            if (!_quiet)
            {
                optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuting });
            }
        }
    }
}
