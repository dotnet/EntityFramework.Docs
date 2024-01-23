using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public abstract class BlogsContext : DbContext
{
    const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=InterceptionTest;Trusted_Connection=True";

    protected BlogsContext()
        : base(
            new DbContextOptionsBuilder()
                .LogTo(Console.WriteLine, LogLevel.Information)
                .UseSqlServer(ConnectionString)
                .Options)
    {
    }

    public DbSet<Blog> Blogs { get; set; }
}

public class Blog
{
    public int Id { get; set; }
    public string Name { get; set; }
}
