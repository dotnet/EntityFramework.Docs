using System;
using Microsoft.EntityFrameworkCore;

public static class TrailingUnderscoresSample
{
    public static void Backing_fields_with_trailing_underscores_are_matched()
    {
        Console.WriteLine($">>>> Sample: {nameof(Backing_fields_with_trailing_underscores_are_matched)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();

        using var context = new SomeDbContext();

        Microsoft.EntityFrameworkCore.Metadata.IEntityType userEntityType = context.Model.FindEntityType(typeof(User));

        Microsoft.EntityFrameworkCore.Metadata.IProperty idProperty = userEntityType.FindProperty(nameof(User.Id));
        Console.WriteLine($"User entity detected backing field '{idProperty.FieldInfo.Name}' for property '{idProperty.Name}'");

        Microsoft.EntityFrameworkCore.Metadata.IProperty nameProperty = userEntityType.FindProperty(nameof(User.Name));
        Console.WriteLine($"User entity detected backing field '{nameProperty.FieldInfo.Name}' for property '{nameProperty.Name}'");

        Console.WriteLine();
    }

    public static class Helpers
    {
        public static void RecreateCleanDatabase()
        {
            using var context = new SomeDbContext(quiet: true);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }

    public class User
    {
        public User(int id, int name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; }
        public int Name { get; }
    }

    public class SomeDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        readonly bool _quiet;

        public SomeDbContext(bool quiet = false) => _quiet = quiet;

        protected override void OnModelCreating(ModelBuilder modelBuilder) =>
            modelBuilder.Entity<User>(
                b =>
                {
                    b.Property(e => e.Id);
                    b.Property(e => e.Name);
                });

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCoreSample");

            if (!_quiet)
            {
                //optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
            }
        }
    }
}
