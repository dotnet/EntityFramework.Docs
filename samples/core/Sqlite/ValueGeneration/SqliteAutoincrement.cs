using Microsoft.EntityFrameworkCore;

namespace EFCore.Sqlite.ValueGeneration;

internal class SqliteAutoincrementContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    #region SqliteAutoincrement
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .Property(b => b.Id)
            .UseAutoincrement();
    }
    #endregion

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Data Source=sample.db");
}

public class Blog
{
    public int Id { get; set; }
    public string Title { get; set; }
}