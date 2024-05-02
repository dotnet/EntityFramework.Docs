namespace NewInEfCore8;

public static class DefaultConstraintSample
{
    public static Task Insert_rows_using_database_default_constraint()
        => DefaultConstraintTest(sqlite: false);

    public static Task Insert_rows_using_database_default_constraint_SQLite()
        => DefaultConstraintTest(sqlite: true);

    public static async Task DefaultConstraintTest(bool sqlite)
    {
        PrintSampleName();

        await using var context = new DatabaseDefaultsContext(useSqlite: sqlite);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        context.ChangeTracker.Clear();

        Console.WriteLine();
        Console.WriteLine("Insert user property with int database default:");

        context.AddRange(new User(), new User { Credits = 77 }, new User { Credits = 0 });
        await context.SaveChangesAsync();

        Console.WriteLine();
        Console.WriteLine("Insert user property with enum database default:");

        context.AddRange(new Course(), new Course { Level = Level.Intermediate }, new Course { Level = Level.Beginner });
        await context.SaveChangesAsync();

        Console.WriteLine();
        Console.WriteLine("Insert user property with bool database default:");

        context.AddRange(new Account(), new Account { IsActive = true }, new Account { IsActive = false });
        await context.SaveChangesAsync();

        Console.WriteLine();
        Console.WriteLine("Insert user property with int database default, sentinel configured:");

        context.AddRange(new UserWithSentinel(), new UserWithSentinel { Credits = 77 }, new UserWithSentinel { Credits = 0 });
        await context.SaveChangesAsync();

        Console.WriteLine();
        Console.WriteLine("Insert user property with enum database default, sentinel configured:");

        context.AddRange(new CourseWithSentinel(), new CourseWithSentinel { Level = Level.Intermediate }, new CourseWithSentinel { Level = Level.Beginner });
        await context.SaveChangesAsync();

        Console.WriteLine();
        Console.WriteLine("Insert user property with bool database default, sentinel configured:");

        context.AddRange(new AccountWithSentinel(), new AccountWithSentinel { IsActive = true }, new AccountWithSentinel { IsActive = false });
        await context.SaveChangesAsync();

        Console.WriteLine();
        Console.WriteLine("Insert user property with int database default, nullable backing field:");

        context.AddRange(new UserWithNullableBackingField(), new UserWithNullableBackingField { Credits = 77 }, new UserWithNullableBackingField { Credits = 0 });
        await context.SaveChangesAsync();

        Console.WriteLine();
        Console.WriteLine("Insert user property with enum database default, nullable backing field:");

        context.AddRange(new CourseWithNullableBackingField(), new CourseWithNullableBackingField { Level = Level.Intermediate }, new CourseWithNullableBackingField { Level = Level.Beginner });
        await context.SaveChangesAsync();

        Console.WriteLine();
        Console.WriteLine("Insert user property with bool database default, nullable backing field:");

        context.AddRange(new AccountWithNullableBackingField(), new AccountWithNullableBackingField { IsActive = true }, new AccountWithNullableBackingField { IsActive = false });
        await context.SaveChangesAsync();

        Console.WriteLine();
    }


    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }

    public class User
    {
        public int Id { get; set; }
        public int Credits { get; set; }
    }

    public class Account
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }

    public class Course
    {
        public int Id { get; set; }
        public Level Level { get; set; }
    }

    public class UserWithSentinel
    {
        public int Id { get; set; }
        public int Credits { get; set; } = -1;
    }

    public class AccountWithSentinel
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class CourseWithSentinel
    {
        public int Id { get; set; }
        public Level Level { get; set; } = Level.Unspecified;
    }

    public class UserWithNullableBackingField
    {
        public int Id { get; set; }

        private int? _credits;

        public int Credits
        {
            get => _credits ?? 0;
            set => _credits = value;
        }
    }

    public class AccountWithNullableBackingField
    {
        public int Id { get; set; }

        private bool? _isActive;

        public bool IsActive
        {
            get => _isActive ?? false;
            set => _isActive = value;
        }
    }

    public class CourseWithNullableBackingField
    {
        public int Id { get; set; }

        private Level? _level;

        public Level Level
        {
            get => _level ?? Level.Unspecified;
            set => _level = value;
        }
    }

    public enum Level
    {
        Beginner,
        Intermediate,
        Advanced,
        Unspecified
    }

    public class DatabaseDefaultsContext(bool useSqlite = false) : DbContext
    {
        public bool UseSqlite { get; } = useSqlite;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => (UseSqlite
                    ? optionsBuilder.UseSqlite(@$"DataSource={GetType().Name}.db")
                    : optionsBuilder.UseSqlServer(
                        @$"Server=(localdb)\mssqllocaldb;Database={GetType().Name};ConnectRetryCount=0",
                        sqlServerOptionsBuilder => sqlServerOptionsBuilder.UseNetTopologySuite()))
                .EnableSensitiveDataLogging()
                .LogTo(Console.WriteLine, LogLevel.Information);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().Property(e => e.IsActive).HasDefaultValueSql("1");
            modelBuilder.Entity<User>().Property(e => e.Credits).HasDefaultValue(100);
            modelBuilder.Entity<Course>().Property(e => e.Level).HasDefaultValue(Level.Intermediate);

            modelBuilder.Entity<AccountWithSentinel>().Property(e => e.IsActive).HasDefaultValueSql("1").HasSentinel(true);
            modelBuilder.Entity<UserWithSentinel>().Property(e => e.Credits).HasDefaultValue(100).HasSentinel(-1);
            modelBuilder.Entity<CourseWithSentinel>().Property(e => e.Level).HasDefaultValue(Level.Intermediate).HasSentinel(Level.Unspecified);

            modelBuilder.Entity<AccountWithNullableBackingField>().Property(e => e.IsActive).HasDefaultValueSql("1");
            modelBuilder.Entity<UserWithNullableBackingField>().Property(e => e.Credits).HasDefaultValue(100);
            modelBuilder.Entity<CourseWithNullableBackingField>().Property(e => e.Level).HasDefaultValue(Level.Intermediate);
        }
    }
}
