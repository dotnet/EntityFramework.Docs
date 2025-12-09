using Microsoft.EntityFrameworkCore;

namespace EFCore.Sqlite.ValueGeneration;

public readonly struct BlogId
{
    public BlogId(int value) => Value = value;
    public int Value { get; }
    
    public static implicit operator int(BlogId id) => id.Value;
    public static implicit operator BlogId(int value) => new(value);
}

public class SqliteAutoincrementWithValueConverterContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    #region SqliteAutoincrementWithValueConverter
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .Property(b => b.Id)
            .HasConversion<int>()
            .UseAutoincrement();
    }
    #endregion

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Data Source=sample.db");
}

public class Blog
{
    public BlogId Id { get; set; }
    public string Title { get; set; }
}