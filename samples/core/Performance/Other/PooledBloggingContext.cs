using Microsoft.EntityFrameworkCore;

namespace Performance.Other;

public class PooledBloggingContext : DbContext
{
    public PooledBloggingContext(DbContextOptions options) : base(options) { }

    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }
}