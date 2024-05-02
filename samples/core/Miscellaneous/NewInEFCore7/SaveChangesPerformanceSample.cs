using Microsoft.EntityFrameworkCore.Diagnostics;

namespace NewInEfCore7;

public static class SaveChangesPerformanceSample
{
    public static Task SaveChanges_SQL_generation_samples_SqlServer()
    {
        PrintSampleName();
        return SaveChangesTest<PerfContextSqlServer>();
    }

    public static Task SaveChanges_SQL_generation_samples_Sqlite()
    {
        PrintSampleName();
        return SaveChangesTest<PerfContextSqlite>();
    }

    private static async Task SaveChangesTest<TContext>()
        where TContext : PerfContext, new()
    {
        await using (var context = new TContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            if (context.Database.IsSqlServer())
            {
                context.Database.ExecuteSqlRaw(
                    @"
CREATE TRIGGER TRG_InsertUpdateBlog
ON BlogsWithTriggers
AFTER INSERT, UPDATE AS
BEGIN
	IF @@ROWCOUNT = 0
		return
	SET nocount on;

    UPDATE BlogsWithTriggers set StoreUpdated = StoreUpdated + 1
    WHERE Id IN(SELECT INSERTED.Id FROM INSERTED);
END");
            }
        }

        await using (var context = new TContext())
        {
            #region SimpleInsert
            await context.AddAsync(new Blog { Name = "MyBlog" });
            await context.SaveChangesAsync();
            #endregion
        }

        await using (var context = new TContext())
        {
            #region MultipleInsert
            for (var i = 0; i < 4; i++)
            {
                await context.AddAsync(new Blog { Name = "Foo" + i });
            }

            await context.SaveChangesAsync();
            #endregion
        }

        await using (var context = new TContext())
        {
            await context.AddAsync(new BlogWithTrigger { Name = "MyBlog" });
            await context.SaveChangesAsync();
        }

        await using (var context = new TContext())
        {
            for (var i = 0; i < 4; i++)
            {
                await context.AddAsync(new BlogWithTrigger { Name = "Foo" + i });
            }

            await context.SaveChangesAsync();
        }

        await using (var context = new TContext())
        {
            #region InsertGraph
            await context.AddAsync(
                new Blog { Name = "MyBlog", Posts = { new() { Title = "My first post" }, new() { Title = "My second post" } } });
            await context.SaveChangesAsync();
            #endregion
        }

        await using (var context = new TContext())
        {
            #region InsertGraph
            await context.AddAsync(
                new Blog { Name = "MyBlog", Posts = { new() { Title = "My first post" }, new() { Title = "My second post" } } });
            await context.SaveChangesAsync();
            #endregion
        }
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }

    public abstract class PerfContext : DbContext
    {
        public DbSet<Blog> Blogs => Set<Blog>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<BlogWithTrigger> BlogsWithTriggers => Set<BlogWithTrigger>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<BlogWithTrigger>()
                .Property(e => e.StoreUpdated)
                .HasDefaultValue(0)
                .ValueGeneratedOnAddOrUpdate();

            if (Database.IsSqlServer())
            {
                #region HasTrigger
                modelBuilder
                    .Entity<BlogWithTrigger>()
                    .ToTable(tb => tb.HasTrigger("TRG_InsertUpdateBlog"));
                #endregion

                #region UseHiLo
                modelBuilder.Entity<Blog>().Property(e => e.Id).UseHiLo();
                modelBuilder.Entity<Post>().Property(e => e.Id).UseHiLo();
                #endregion
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .LogTo(
                    Console.WriteLine,
                    new[]
                    {
                        RelationalEventId.CommandExecuted, RelationalEventId.TransactionStarted, RelationalEventId.TransactionCommitted
                    })
                .EnableSensitiveDataLogging();
    }

    public class PerfContextSqlServer : PerfContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => base.OnConfiguring(
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=SaveChangesPerf;ConnectRetryCount=0"));
    }

    public class PerfContextSqlite : PerfContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => base.OnConfiguring(
                optionsBuilder.UseSqlite("Data Source = savechangesperf.db"));
    }

    public class Blog
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public List<Post> Posts { get; } = new();
    }

    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public Blog Blog { get; set; } = default!;
    }

    public class BlogWithTrigger
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public virtual int StoreUpdated { get; set; }
    }
}
