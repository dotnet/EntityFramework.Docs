using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public static class EntityTypeConfigurationAttributeSample
{
    public static async Task Using_EntityTypeConfigurationAttribute()
    {
        Console.WriteLine($">>>> Sample: {nameof(Using_EntityTypeConfigurationAttribute)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new BooksContext();

        var book = await context.Books.SingleAsync(e => e.Id == 1);

        Console.WriteLine();
    }

    public static class Helpers
    {
        public static async Task RecreateCleanDatabase()
        {
            using var context = new BooksContext();

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        public static async Task PopulateDatabase()
        {
            using var context = new BooksContext();

            context.Add(
                new Book() { Isbn = "978-0-39-481823-8", Title = "What Do People Do All Day?" });

            await context.SaveChangesAsync();
        }
    }

    #region BookEntityType
    [EntityTypeConfiguration(typeof(BookConfiguration))]
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Isbn { get; set; }
    }
    #endregion

    #region BookConfiguration
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder
                .Property(e => e.Isbn)
                .IsUnicode(false)
                .HasMaxLength(22);
        }
    }
    #endregion

    #region DbContext
    public class BooksContext : DbContext
    {
        public DbSet<Book> Books { get; set; }

        //...
        #endregion

        private readonly bool _quiet;

        public BooksContext(bool quiet = false)
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
                optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
            }
        }
    }
}
