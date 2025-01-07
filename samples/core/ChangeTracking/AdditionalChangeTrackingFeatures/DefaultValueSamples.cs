using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DefaultValues;

public class DefaultValueSamples
{
    public static async Task Working_with_default_values_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Working_with_default_values_1)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();

        #region Working_with_default_values_1
        using var context = new BlogsContext();

        context.AddRange(
            new Token { Name = "A" },
            new Token { Name = "B", ValidFrom = new DateTime(1111, 11, 11, 11, 11, 11) });

        await context.SaveChangesAsync();

        Console.WriteLine(context.ChangeTracker.DebugView.LongView);
        #endregion

        Console.WriteLine();
    }

    public static async Task Working_with_default_values_2()
    {
        Console.WriteLine($">>>> Sample: {nameof(Working_with_default_values_2)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();

        #region Working_with_default_values_2
        using var context = new BlogsContext();

        var fooA = new Foo1 { Count = 10 };
        var fooB = new Foo1 { Count = 0 };
        var fooC = new Foo1();

        context.AddRange(fooA, fooB, fooC);
        await context.SaveChangesAsync();

        Debug.Assert(fooA.Count == 10);
        Debug.Assert(fooB.Count == -1); // Not what we want!
        Debug.Assert(fooC.Count == -1);
        #endregion

        Console.WriteLine();
    }

    public static async Task Working_with_default_values_3()
    {
        Console.WriteLine($">>>> Sample: {nameof(Working_with_default_values_3)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();

        #region Working_with_default_values_3
        using var context = new BlogsContext();

        var fooA = new Foo2 { Count = 10 };
        var fooB = new Foo2 { Count = 0 };
        var fooC = new Foo2();

        context.AddRange(fooA, fooB, fooC);
        await context.SaveChangesAsync();

        Debug.Assert(fooA.Count == 10);
        Debug.Assert(fooB.Count == 0);
        Debug.Assert(fooC.Count == -1);
        #endregion

        Console.WriteLine();
    }

    public static async Task Working_with_default_values_4()
    {
        Console.WriteLine($">>>> Sample: {nameof(Working_with_default_values_4)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();

        #region Working_with_default_values_4
        using var context = new BlogsContext();

        var fooA = new Foo3 { Count = 10 };
        var fooB = new Foo3 { Count = 0 };
        var fooC = new Foo3();

        context.AddRange(fooA, fooB, fooC);
        await context.SaveChangesAsync();

        Debug.Assert(fooA.Count == 10);
        Debug.Assert(fooB.Count == 0);
        Debug.Assert(fooC.Count == -1);
        #endregion

        Console.WriteLine();
    }

    public static async Task Working_with_default_values_5()
    {
        Console.WriteLine($">>>> Sample: {nameof(Working_with_default_values_5)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();

        #region Working_with_default_values_5
        using var context = new BlogsContext();

        var userA = new User { Name = "Mac" };
        var userB = new User { Name = "Alice", IsAuthorized = true };
        var userC = new User { Name = "Baxter", IsAuthorized = false }; // Always deny Baxter access!

        context.AddRange(userA, userB, userC);

        await context.SaveChangesAsync();
        #endregion

        Console.WriteLine();
    }
}

public static class Helpers
{
    public static async Task RecreateCleanDatabase()
    {
        using var context = new BlogsContext(quiet: true);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}

#region Token
public class Token
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime ValidFrom { get; set; }
}
#endregion

#region Foo1
public class Foo1
{
    public int Id { get; set; }
    public int Count { get; set; }
}
#endregion

#region Foo2
public class Foo2
{
    public int Id { get; set; }
    public int? Count { get; set; }
}
#endregion

#region Foo3
public class Foo3
{
    public int Id { get; set; }

    private int? _count;

    public int Count
    {
        get => _count ?? -1;
        set => _count = value;
    }
}
#endregion

#region Bar
public class Bar
{
    public int Id { get; set; }
    public int Count { get; set; }
}
#endregion

#region User
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }

    private bool? _isAuthorized;

    public bool IsAuthorized
    {
        get => _isAuthorized ?? true;
        set => _isAuthorized = value;
    }
}
#endregion

public class BlogsContext : DbContext
{
    private readonly bool _quiet;

    public BlogsContext(bool quiet = false)
    {
        _quiet = quiet;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .EnableSensitiveDataLogging()
            .UseSqlite("DataSource=test.db");

        if (!_quiet)
        {
            optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region OnModelCreating_Token
        modelBuilder
            .Entity<Token>()
            .Property(e => e.ValidFrom)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        #endregion

        #region OnModelCreating_Foo1
        modelBuilder
            .Entity<Foo1>()
            .Property(e => e.Count)
            .HasDefaultValue(-1);
        #endregion

        #region OnModelCreating_Foo2
        modelBuilder
            .Entity<Foo2>()
            .Property(e => e.Count)
            .HasDefaultValue(-1);
        #endregion

        #region OnModelCreating_Foo3
        modelBuilder
            .Entity<Foo3>()
            .Property(e => e.Count)
            .HasDefaultValue(-1);
        #endregion

        #region OnModelCreating_User
        modelBuilder
            .Entity<User>()
            .Property(e => e.IsAuthorized)
            .HasDefaultValue(true);
        #endregion

        #region OnModelCreating_Bar
        modelBuilder
            .Entity<Bar>()
            .Property(e => e.Count)
            .HasDefaultValue(-1)
            .ValueGeneratedNever();
        #endregion
    }
}
