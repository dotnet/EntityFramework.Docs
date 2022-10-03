using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace NewInEfCore7;

public static class DbContextApiSample
{
    public static async Task Find_siblings()
    {
        PrintSampleName();

        await using var context = new BlogsContext();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.Seed();
        context.ChangeTracker.Clear();

        var blogs = await context.Blogs.Include(blog => blog.Posts).ToListAsync();
        var post = blogs.First().Posts.First();

        #region UseFindSiblings

        Console.WriteLine($"Siblings to {post.Id}: '{post.Title}' are...");
        foreach (var sibling in context.FindSiblings(post, nameof(post.Blog)))
        {
            Console.WriteLine($"    {sibling.Id}: '{sibling.Title}'");
        }

        #endregion
    }

    public static async Task Get_entry_for_shared_type_entity_type()
    {
        PrintSampleName();

        await using (var context = new BlogsContext { LoggingEnabled = true })
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            var builds = new List<Dictionary<string, object>>
            {
                new()
                {
                    { "Tag", "v6.0.9" },
                    { "Version", new Version(6, 0, 9) },
                    { "Prerelease", false },
                    { "Hash", "33e3c950af2eb996c0b3c48e30eb4471138da675" }
                },
                new()
                {
                    { "Tag", "v3.1.29" },
                    { "Version", new Version(3, 1, 29) },
                    { "Prerelease", false },
                    { "Hash", "6e3b6bb213c8360c3a1e26f91c66234710535962" }
                },
                new()
                {
                    { "Tag", "v7.0.0-rc.1.22426.7" },
                    { "Version", new Version(7, 0, 0) },
                    { "Prerelease", true },
                    { "Hash", "dc0f3e8ef10eb1464b27f0fd4704f53c01226036" }
                }
            };

            ListBuilds(context, builds);

            await context.BuildMetadata.AddRangeAsync(builds);
            ListBuilds(context, builds);

            await context.SaveChangesAsync();
            ListBuilds(context, builds);
        }

        await using (var context = new BlogsContext { LoggingEnabled = true })
        {
            #region BuildMetadataQuery
            var builds = await context.BuildMetadata
                .Where(metadata => !EF.Property<bool>(metadata, "Prerelease"))
                .OrderBy(metadata => EF.Property<string>(metadata, "Tag"))
                .ToListAsync();
            #endregion

            ListBuilds(context, builds);
        }

        void ListBuilds(BlogsContext context, List<Dictionary<string, object>> builds)
        {
            Console.WriteLine();
            Console.WriteLine("Builds:");

            foreach (var build in builds)
            {
                #region GetEntry
                var state = context.BuildMetadata.Entry(build).State;
                #endregion

                Console.WriteLine(
                    $"  Build {build["Tag"]} for {build["Version"]} with hash '{build["Hash"]}' with state '{state}'");
            }

            Console.WriteLine();
        }
    }

    public static async Task Use_IEntityEntryGraphIterator()
    {
        PrintSampleName();

        await using var context = new BlogsContext();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.Seed();

        #region IEntityEntryGraphIterator
        var blogEntry = context.ChangeTracker.Entries<Blog>().First();
        var found = new HashSet<object>();
        var iterator = context.GetService<IEntityEntryGraphIterator>();
        iterator.TraverseGraph(new EntityEntryGraphNode<HashSet<object>>(blogEntry, found, null, null), node =>
        {
            if (node.NodeState.Contains(node.Entry.Entity))
            {
                return false;
            }

            Console.Write($"Found with '{node.Entry.Entity.GetType().Name}'");

            if (node.InboundNavigation != null)
            {
                Console.Write($" by traversing '{node.InboundNavigation.Name}' from '{node.SourceEntry!.Entity.GetType().Name}'");
            }

            Console.WriteLine();

            node.NodeState.Add(node.Entry.Entity);

            return true;
        });

        Console.WriteLine();
        Console.WriteLine($"Finished iterating. Found {found.Count} entities.");
        Console.WriteLine();
        #endregion
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }

    public class BlogsContext : ModelBuildingBlogsContextBase
    {
        #region BuildMetadataSet
        public DbSet<Dictionary<string, object>> BuildMetadata
            => Set<Dictionary<string, object>>("BuildMetadata");
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            #region ContextInitializedLog
            optionsBuilder.ConfigureWarnings(
                builder =>
                {
                    builder.Log((CoreEventId.ContextInitialized, LogLevel.Information));
                });
            #endregion

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region BuildMetadata
            modelBuilder.SharedTypeEntity<Dictionary<string, object>>(
                "BuildMetadata", b =>
                {
                    b.IndexerProperty<int>("Id");
                    b.IndexerProperty<string>("Tag");
                    b.IndexerProperty<Version>("Version");
                    b.IndexerProperty<string>("Hash");
                    b.IndexerProperty<bool>("Prerelease");
                });
            #endregion
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);

            configurationBuilder.Properties<Version>().HaveConversion<VersionConverter>();
        }

        private class VersionConverter : ValueConverter<Version, string>
        {
            public VersionConverter()
                : base(v => v.ToString(), v => new Version(v))
            {
            }
        }
    }
}

public static class BlogsContextExtensions
{
    #region FindSiblings
    public static IEnumerable<TEntity> FindSiblings<TEntity>(
        this DbContext context, TEntity entity, string navigationToParent)
        where TEntity : class
    {
        var parentEntry = context.Entry(entity).Reference(navigationToParent);

        return context.Entry(parentEntry.CurrentValue!)
            .Collection(parentEntry.Metadata.Inverse!)
            .CurrentValue!
            .OfType<TEntity>()
            .Where(e => !ReferenceEquals(e, entity));
    }
    #endregion
}
