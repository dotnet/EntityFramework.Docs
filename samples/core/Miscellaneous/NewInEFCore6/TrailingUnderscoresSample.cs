using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public static class TrailingUnderscoresSample
{
    public static async Task Backing_fields_with_trailing_underscores_are_matched()
    {
        Console.WriteLine($">>>> Sample: {nameof(Backing_fields_with_trailing_underscores_are_matched)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();

        using var context = new SomeDbContext();

        var userEntityType = context.Model.FindEntityType(typeof(User));

        var idProperty = userEntityType.FindProperty(nameof(User.Id));
        Console.WriteLine($"User entity detected backing field '{idProperty.FieldInfo.Name}' for property '{idProperty.Name}'");

        var nameProperty = userEntityType.FindProperty(nameof(User.Name));
        Console.WriteLine($"User entity detected backing field '{nameProperty.FieldInfo.Name}' for property '{nameProperty.Name}'");

        Console.WriteLine();
    }

    public static class Helpers
    {
        public static async Task RecreateCleanDatabase()
        {
            using var context = new SomeDbContext(quiet: true);

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }
    }

    public class User
    {
        private readonly int id_;
        private readonly int name_;

        public User(int id, int name)
        {
            id_ = id;
            name_ = name;
        }

        public int Id => id_;
        public int Name => name_;
    }

    public class SomeDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        private readonly bool _quiet;

        public SomeDbContext(bool quiet = false)
        {
            _quiet = quiet;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(
                b =>
                {
                    b.Property(e => e.Id);
                    b.Property(e => e.Name);
                });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCoreSample;ConnectRetryCount=0");

            if (!_quiet)
            {
                //optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
            }
        }
    }
}
