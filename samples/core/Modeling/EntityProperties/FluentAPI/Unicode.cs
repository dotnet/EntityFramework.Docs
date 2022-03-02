using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityProperties.FluentAPI.Unicode;

internal class MyContext : DbContext
{
    public DbSet<Book> Books { get; set; }

    #region Unicode
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>()
            .Property(b => b.Isbn)
            .IsUnicode(false);
    }
    #endregion
}

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Isbn { get; set; }
}