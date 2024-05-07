namespace NewInEfCore7;

public static class ModelBuildingSample
{
    public static async Task Indexes_can_be_ordered()
    {
        PrintSampleName();

        await using var context = new BlogsContext();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    public static async Task Property_can_be_mapped_to_different_column_names_TPT()
    {
        PrintSampleName();

        await using var context = new AnimalsTptContext();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    public static async Task Property_can_be_mapped_to_different_column_names_TPC()
    {
        PrintSampleName();

        await using var context = new AnimalsTpcContext();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    public static async Task Entity_splitting()
    {
        PrintSampleName();

        await using (var context = new EntitySplittingContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            await context.AddRangeAsync(
                new Customer("Alice", "1 Main St", "Chigley", "CW1 5ZH", "UK") { PhoneNumber = "01632 12348" },
                new Customer("Mac", "2 Main St", "Chigley", "CW1 5ZH", "UK"),
                new Customer("Toast", "3 Main St", "Chigley", null, "UK") { PhoneNumber = "01632 12348" });

            await context.SaveChangesAsync();
        }

        await using (var context = new EntitySplittingContext())
        {
            var customers = await context.Customers
                .Where(customer => customer.PhoneNumber!.StartsWith("01632") && customer.City == "Chigley")
                .OrderBy(customer => customer.PostCode)
                .ThenBy(customer => customer.Name)
                .ToListAsync();

            foreach (var customer in customers)
            {
                Console.WriteLine($"{customer.Name} from {customer.City} with phone {customer.PhoneNumber}");
            }
        }
    }

    public static async Task Temporal_tables_with_owned_types()
    {
        PrintSampleName();

        DateTime timeStamp1;
        DateTime timeStamp2;
        DateTime timeStamp3;
        DateTime timeStamp4;

        await using (var context = new EntitySplittingContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            await context.AddRangeAsync(
                new Employee
                {
                    Name = "Pinky Pie",
                    Info = new()
                    {
                        Address = "Sugarcube Corner, Ponyville, Equestria",
                        Department = "DevDiv",
                        Position = "Party Organizer",
                        AnnualSalary = 100.0m
                    }
                },
                new Employee
                {
                    Name = "Rainbow Dash",
                    Info = new()
                    {
                        Address = "Cloudominium, Ponyville, Equestria",
                        Department = "DevDiv",
                        Position = "Ponyville weather patrol",
                        AnnualSalary = 900.0m
                    }
                },
                new Employee
                {
                    Name = "Fluttershy",
                    Info = new()
                    {
                        Address = "Everfree Forest, Equestria",
                        Department = "DevDiv",
                        Position = "Animal caretaker",
                        AnnualSalary = 30.0m
                    }
                });

            await context.SaveChangesAsync();
        }

        await using (var context = new EntitySplittingContext())
        {
            Console.WriteLine();
            Console.WriteLine("Starting data:");

            var employees = await context.Employees.ToListAsync();
            foreach (var employee in employees)
            {
                var employeeEntry = context.Entry(employee);
                var periodStart = employeeEntry.Property<DateTime>("PeriodStart").CurrentValue;
                var periodEnd = employeeEntry.Property<DateTime>("PeriodEnd").CurrentValue;

                Console.WriteLine($"  Employee {employee.Name} valid from {periodStart} to {periodEnd}");
            }
        }

        await using (var context = new EntitySplittingContext())
        {
            // Change the sleep values to emphasize the temporal nature of the data.
            const int millisecondsDelay = 100;

            Thread.Sleep(millisecondsDelay);
            timeStamp1 = DateTime.UtcNow;
            Thread.Sleep(millisecondsDelay);

            var employee = await context.Employees.SingleAsync(e => e.Name == "Rainbow Dash");
            employee.Info.Position = "Wonderbolt Trainee";
            await context.SaveChangesAsync();

            Thread.Sleep(millisecondsDelay);
            timeStamp2 = DateTime.UtcNow;
            Thread.Sleep(millisecondsDelay);

            employee.Info.Position = "Wonderbolt Reservist";
            await context.SaveChangesAsync();

            Thread.Sleep(millisecondsDelay);
            timeStamp3 = DateTime.UtcNow;
            Thread.Sleep(millisecondsDelay);

            employee.Info.Position = "Wonderbolt";
            await context.SaveChangesAsync();

            Thread.Sleep(millisecondsDelay);
            timeStamp4 = DateTime.UtcNow;
            Thread.Sleep(millisecondsDelay);
        }

        await using (var context = new EntitySplittingContext())
        {
            #region NormalQuery
            var employee = await context.Employees.SingleAsync(e => e.Name == "Rainbow Dash");
            context.Remove(employee);
            await context.SaveChangesAsync();
            #endregion
        }

        await using (var context = new EntitySplittingContext())
        {
            Console.WriteLine();
            Console.WriteLine("After updates and delete:");

            #region TrackingQuery
            var employees = await context.Employees.ToListAsync();
            foreach (var employee in employees)
            {
                var employeeEntry = context.Entry(employee);
                var periodStart = employeeEntry.Property<DateTime>("PeriodStart").CurrentValue;
                var periodEnd = employeeEntry.Property<DateTime>("PeriodEnd").CurrentValue;

                Console.WriteLine($"  Employee {employee.Name} valid from {periodStart} to {periodEnd}");
            }
            #endregion

            Console.WriteLine();
            Console.WriteLine("Historical data for Rainbow Dash:");

            #region TemporalAll
            var history = await context
                .Employees
                .TemporalAll()
                .Where(e => e.Name == "Rainbow Dash")
                .OrderBy(e => EF.Property<DateTime>(e, "PeriodStart"))
                .Select(
                    e => new
                    {
                        Employee = e,
                        PeriodStart = EF.Property<DateTime>(e, "PeriodStart"),
                        PeriodEnd = EF.Property<DateTime>(e, "PeriodEnd")
                    })
                .ToListAsync();

            foreach (var pointInTime in history)
            {
                Console.WriteLine(
                    $"  Employee {pointInTime.Employee.Name} was '{pointInTime.Employee.Info.Position}' from {pointInTime.PeriodStart} to {pointInTime.PeriodEnd}");
            }
            #endregion
        }

        await using (var context = new EntitySplittingContext())
        {
            Console.WriteLine();
            Console.WriteLine($"Historical data for Rainbow Dash between {timeStamp2} and {timeStamp3}:");

            #region TemporalBetween
            var history = await context
                .Employees
                .TemporalBetween(timeStamp2, timeStamp3)
                .Where(e => e.Name == "Rainbow Dash")
                .OrderBy(e => EF.Property<DateTime>(e, "PeriodStart"))
                .Select(
                    e => new
                    {
                        Employee = e,
                        PeriodStart = EF.Property<DateTime>(e, "PeriodStart"),
                        PeriodEnd = EF.Property<DateTime>(e, "PeriodEnd")
                    })
                .ToListAsync();
            #endregion

            foreach (var pointInTime in history)
            {
                Console.WriteLine(
                    $"  Employee {pointInTime.Employee.Name} was '{pointInTime.Employee.Info.Position}' from {pointInTime.PeriodStart} to {pointInTime.PeriodEnd}");
            }
        }

        await using (var context = new EntitySplittingContext())
        {
            Console.WriteLine();
            Console.WriteLine($"Historical data for Rainbow Dash as of {timeStamp2}:");

            var history = await context
                .Employees
                .TemporalAsOf(timeStamp2)
                .Where(e => e.Name == "Rainbow Dash")
                .OrderBy(e => EF.Property<DateTime>(e, "PeriodStart"))
                .Select(
                    e => new
                    {
                        Employee = e, PeriodStart = EF.Property<DateTime>(e, "PeriodStart"), PeriodEnd = EF.Property<DateTime>(e, "PeriodEnd")
                    })
                .ToListAsync();

            foreach (var pointInTime in history)
            {
                Console.WriteLine(
                    $"  Employee {pointInTime.Employee.Name} was '{pointInTime.Employee.Info.Position}' from {pointInTime.PeriodStart} to {pointInTime.PeriodEnd}");
            }
        }
    }

    public static async Task Unidirectional_many_to_many()
    {
        PrintSampleName();

        await using var context = new BlogsContext();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        #region InsertPostsAndTags
        var tags = new Tag[] { new() { TagName = "Tag1" }, new() { TagName = "Tag2" }, new() { TagName = "Tag2" }, };

        await context.AddRangeAsync(new Blog { Posts =
        {
            new Post { Tags = { tags[0], tags[1] } },
            new Post { Tags = { tags[1], tags[0], tags[2] } },
            new Post()
        } });

        await context.SaveChangesAsync();
        #endregion
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }

    public class BlogsContext : DbContext
    {
        public DbSet<Blog> Blogs => Set<Blog>();
        public DbSet<Post> Posts => Set<Post>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseSqlServer(@$"Server=(localdb)\mssqllocaldb;Database=Blogs;ConnectRetryCount=0")
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region CompositeIndex
            modelBuilder
                .Entity<Blog>()
                .HasIndex(blog => new { blog.Name, blog.Owner })
                .IsDescending(false, true);
            #endregion

            #region TwoIndexes
            modelBuilder
                .Entity<Blog>()
                .HasIndex(blog => new { blog.Name, blog.Owner }, "IX_Blogs_Name_Owner_1")
                .IsDescending(false, true);

            modelBuilder
                .Entity<Blog>()
                .HasIndex(blog => new { blog.Name, blog.Owner }, "IX_Blogs_Name_Owner_2")
                .IsDescending(true, true);
            #endregion

            #region ManyToMany
            modelBuilder
                .Entity<Post>()
                .HasMany(post => post.Tags)
                .WithMany();
            #endregion

            #region Utf8
            modelBuilder
                .Entity<Comment>()
                .Property(comment => comment.CommentText)
                .HasColumnType("varchar(max)")
                .UseCollation("LATIN1_GENERAL_100_CI_AS_SC_UTF8");
            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }

    #region CompositeIndexByAttribute
    [Index(nameof(Name), nameof(Owner), IsDescending = new[] { false, true }, Name = "IX_Blogs_Name_Owner_1")]
    [Index(nameof(Name), nameof(Owner), IsDescending = new[] { true, true }, Name = "IX_Blogs_Name_Owner_2")]
    public class Blog
    {
        public int Id { get; set; }

        [MaxLength(64)]
        public string? Name { get; set; }

        [MaxLength(64)]
        public string? Owner { get; set; }

        public List<Post> Posts { get; } = new();
    }
    #endregion

    public class Post
    {
        public int Id { get; set; }

        [MaxLength(64)]
        public string? Title { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Blog Blog { get; set; } = null!;

        public List<Tag> Tags { get; } = new();
    }

    #region CompositePrimaryKey
    [PrimaryKey(nameof(PostId), nameof(CommentId))]
    public class Comment
    {
        public int PostId { get; set; }
        public int CommentId { get; set; }
        public string CommentText { get; set; } = null!;
    }
    #endregion

    #region Tag
    public class Tag
    {
        public int Id { get; set; }
        public string TagName { get; set; } = null!;
    }
    #endregion

    public class AnimalsTptContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseSqlServer(@$"Server=(localdb)\mssqllocaldb;Database=AnimalsTpt;ConnectRetryCount=0")
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region AnimalsTpt
            modelBuilder.Entity<Animal>().ToTable("Animals");

            modelBuilder.Entity<Cat>()
                .ToTable(
                    "Cats",
                    tableBuilder => tableBuilder.Property(cat => cat.Id).HasColumnName("CatId"));

            modelBuilder.Entity<Dog>()
                .ToTable(
                    "Dogs",
                    tableBuilder => tableBuilder.Property(dog => dog.Id).HasColumnName("DogId"));
            #endregion
        }
    }

    public class AnimalsTpcContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseSqlServer(@$"Server=(localdb)\mssqllocaldb;Database=AnimalsTpc;ConnectRetryCount=0")
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region AnimalsTpc
            modelBuilder.Entity<Animal>().UseTpcMappingStrategy();

            modelBuilder.Entity<Cat>()
                .ToTable(
                    "Cats",
                    builder =>
                    {
                        builder.Property(cat => cat.Id).HasColumnName("CatId");
                        builder.Property(cat => cat.Breed).HasColumnName("CatBreed");
                    });

            modelBuilder.Entity<Dog>()
                .ToTable(
                    "Dogs",
                    builder =>
                    {
                        builder.Property(dog => dog.Id).HasColumnName("DogId");
                        builder.Property(dog => dog.Breed).HasColumnName("DogBreed");
                    });
            #endregion
        }
    }

    #region Animals
    public abstract class Animal
    {
        public int Id { get; set; }
        public string Breed { get; set; } = null!;
    }

    public class Cat : Animal
    {
        public string? EducationalLevel { get; set; }
    }

    public class Dog : Animal
    {
        public string? FavoriteToy { get; set; }
    }
    #endregion

    public class EntitySplittingContext : DbContext
    {
        public DbSet<Customer> Customers
            => Set<Customer>();

        public DbSet<Employee> Employees
            => Set<Employee>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseSqlServer(@$"Server=(localdb)\mssqllocaldb;Database=Images;ConnectRetryCount=0")
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region EntitySplitting
            modelBuilder.Entity<Customer>(
                entityBuilder =>
                {
                    entityBuilder
                        .ToTable("Customers")
                        .SplitToTable(
                            "PhoneNumbers",
                            tableBuilder =>
                            {
                                tableBuilder.Property(customer => customer.Id).HasColumnName("CustomerId");
                                tableBuilder.Property(customer => customer.PhoneNumber);
                            })
                        .SplitToTable(
                            "Addresses",
                            tableBuilder =>
                            {
                                tableBuilder.Property(customer => customer.Id).HasColumnName("CustomerId");
                                tableBuilder.Property(customer => customer.Street);
                                tableBuilder.Property(customer => customer.City);
                                tableBuilder.Property(customer => customer.PostCode);
                                tableBuilder.Property(customer => customer.Country);
                            });
                });
            #endregion

            #region LinkingForeignKey
            modelBuilder.Entity<Customer>()
                .HasOne<Customer>()
                .WithOne()
                .HasForeignKey<Customer>(a => a.Id)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region OwnedTemporalTable
            modelBuilder
                .Entity<Employee>()
                .ToTable(
                    "Employees",
                    tableBuilder =>
                    {
                        tableBuilder.IsTemporal();
                        tableBuilder.Property<DateTime>("PeriodStart").HasColumnName("PeriodStart");
                        tableBuilder.Property<DateTime>("PeriodEnd").HasColumnName("PeriodEnd");
                    })
                .OwnsOne(
                    employee => employee.Info,
                    ownedBuilder => ownedBuilder.ToTable(
                        "Employees",
                        tableBuilder =>
                        {
                            tableBuilder.IsTemporal();
                            tableBuilder.Property<DateTime>("PeriodStart").HasColumnName("PeriodStart");
                            tableBuilder.Property<DateTime>("PeriodEnd").HasColumnName("PeriodEnd");
                        }));
            #endregion
        }
    }

    #region CombinedCustomer
    public class Customer
    {
        public Customer(string name, string street, string city, string? postCode, string country)
        {
            Name = name;
            Street = street;
            City = city;
            PostCode = postCode;
            Country = country;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string? PostCode { get; set; }
        public string Country { get; set; }
    }
    #endregion

    #region EmployeeAndEmployeeInfo
    public class Employee
    {
        public Guid EmployeeId { get; set; }
        public string Name { get; set; } = null!;

        public EmployeeInfo Info { get; set; } = null!;
    }

    public class EmployeeInfo
    {
        public string Position { get; set; } = null!;
        public string Department { get; set; } = null!;
        public string? Address { get; set; }
        public decimal? AnnualSalary { get; set; }
    }
    #endregion
}
