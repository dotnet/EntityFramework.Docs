using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static async Task Main()
    {
        using (var context = new TaggedQueryCommandInterceptorContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            context.AddRange(
                new Blog { Name = "Blog1" },
                new Blog { Name = "Blog2" });

            await context.SaveChangesAsync();
        }

        using (var context = new TaggedQueryCommandInterceptorContext())
        {
            #region TaggedQuery
            var blogs1 = await context.Blogs.TagWith("Use hint: robust plan").ToListAsync();
            #endregion
            var blogs2 = await context.Blogs.ToListAsync();
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
