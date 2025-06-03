using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

public class Program
{
    public static async Task Main()
    {
        #region Demonstration
        using (var context = new BlogsContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            context.Add(
                new Blog
                {
                    Id = 1,
                    Name = "EF Blog",
                    Posts = { new Post { Id = 1, Title = "EF Core 3.1!" }, new Post { Id = 2, Title = "EF Core 5.0!" } }
                });

            await context.SaveChangesAsync();
        }

        using (var context = new BlogsContext())
        {
            var blog = await context.Blogs.Include(e => e.Posts).SingleAsync();

            blog.Name = "EF Core Blog";
            context.Remove(blog.Posts.First());
            blog.Posts.Add(new Post { Id = 3, Title = "EF Core 6.0!" });

            await context.SaveChangesAsync();
        }
        #endregion
    }
}

#region BlogsContext
public class BlogsContext : DbContext
{
    #region ContextConstructor
    public BlogsContext()
    {
        ChangeTracker.StateChanged += UpdateTimestamps;
        ChangeTracker.Tracked += UpdateTimestamps;
    }
    #endregion

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("DataSource=blogs.db");

    public DbSet<Blog> Blogs { get; set; }

    #region UpdateTimestamps
    private static void UpdateTimestamps(object sender, EntityEntryEventArgs e)
    {
        if (e.Entry.Entity is IHasTimestamps entityWithTimestamps)
        {
            switch (e.Entry.State)
            {
                case EntityState.Deleted:
                    entityWithTimestamps.Deleted = DateTime.UtcNow;
                    Console.WriteLine($"Stamped for delete: {e.Entry.Entity}");
                    break;
                case EntityState.Modified:
                    entityWithTimestamps.Modified = DateTime.UtcNow;
                    Console.WriteLine($"Stamped for update: {e.Entry.Entity}");
                    break;
                case EntityState.Added:
                    entityWithTimestamps.Added = DateTime.UtcNow;
                    Console.WriteLine($"Stamped for insert: {e.Entry.Entity}");
                    break;
            }
        }
    }
    #endregion
}

public static class HasTimestampsExtensions
{
    public static string ToStampString(this IHasTimestamps entity)
    {
        return $"{GetStamp("Added", entity.Added)}{GetStamp("Modified", entity.Modified)}{GetStamp("Deleted", entity.Deleted)}";

        string GetStamp(string state, DateTime? dateTime)
            => dateTime == null ? "" : $" {state} on: {dateTime}";
    }
}

#region IHasTimestamps
public interface IHasTimestamps
{
    DateTime? Added { get; set; }
    DateTime? Deleted { get; set; }
    DateTime? Modified { get; set; }
}
#endregion

public class Blog : IHasTimestamps
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    public string Name { get; set; }

    public ICollection<Post> Posts { get; } = new List<Post>();

    public DateTime? Added { get; set; }
    public DateTime? Deleted { get; set; }
    public DateTime? Modified { get; set; }

    public override string ToString()
        => $"Blog {Id}{this.ToStampString()}";
}

public class Post : IHasTimestamps
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    public string Title { get; set; }

    public Blog Blog { get; set; }

    public DateTime? Added { get; set; }
    public DateTime? Deleted { get; set; }
    public DateTime? Modified { get; set; }

    public override string ToString()
        => $"Post {Id}{this.ToStampString()}";
}
#endregion
