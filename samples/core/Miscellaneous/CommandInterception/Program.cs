using System.Linq;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static void Main()
    {
        using (var context = new TaggedQueryCommandInterceptorContext())
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.AddRange(
                new Blog { Name = "Blog1" },
                new Blog { Name = "Blog2" });

            context.SaveChanges();
        }

        using (var context = new TaggedQueryCommandInterceptorContext())
        {
            #region TaggedQuery
            var blogs1 = context.Blogs.TagWith("Use hint: robust plan").ToList();
            #endregion
            var blogs2 = context.Blogs.ToList();
        }
    }
}

#region RegisterStatelessInterceptor
public class TaggedQueryCommandInterceptorContext : BlogsContext
{
    private static readonly TaggedQueryCommandInterceptor _interceptor
        = new TaggedQueryCommandInterceptor();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.AddInterceptors(_interceptor);
}
#endregion

#region RegisterInterceptor
public class ExampleContext : BlogsContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.AddInterceptors(new TaggedQueryCommandInterceptor());
}
#endregion
