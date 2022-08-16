namespace NewInEfCore7;

#region BlogsModel
public class Blog
{
    public Blog(string name)
    {
        Name = name;
    }

    public int Id { get; private set; }
    public string Name { get; set; }
    public List<Post> Posts { get; } = new();
}

public class Post
{
    public Post(string title, string content, DateTime publishedOn)
    {
        Title = title;
        Content = content;
        PublishedOn = publishedOn;
    }

    public int Id { get; private set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime PublishedOn { get; set; }
    public Blog Blog { get; set; } = null!;
    public List<Tag> Tags { get; } = new();
    public Author? Author { get; set; }
}

public class FeaturedPost : Post
{
    public FeaturedPost(string title, string content, DateTime publishedOn, string promoText)
        : base(title, content, publishedOn)
    {
        PromoText = promoText;
    }

    public string PromoText { get; set; }
}

public class Tag
{
    public Tag(string text)
    {
        Text = text;
    }

    public int Id { get; private set; }
    public string Text { get; set; }
    public List<Post> Posts { get; } = new();
}

public class Author
{
    public Author(string name)
    {
        Name = name;
    }

    public int Id { get; private set; }
    public string Name { get; set; }
    public ContactDetails Contact { get; set; } = null!;
    public List<Post> Posts { get; } = new();
}

public class ContactDetails
{
    public Address Address { get; init; } = null!;
    public string? Phone { get; init; }
}

public class Address
{
    public Address(string street, string city, string postcode, string country)
    {
        Street = street;
        City = city;
        Postcode = postcode;
        Country = country;
    }

    public string Street { get; init; }
    public string City { get; init; }
    public string Postcode { get; init; }
    public string Country { get; init; }
}
#endregion

public abstract class BlogsContext : DbContext
{
    protected BlogsContext(bool useSqlite)
    {
        UseSqlite = useSqlite;
    }

    public bool UseSqlite { get; }
    public bool LoggingEnabled { get; set; }
    public abstract MappingStrategy MappingStrategy { get; }

    public DbSet<Blog> Blogs => Set<Blog>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Author> Authors => Set<Author>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => (UseSqlite
                ? optionsBuilder.UseSqlite(@$"DataSource={GetType().Name}")
                : optionsBuilder.UseSqlServer(@$"Server=(localdb)\mssqllocaldb;Database={GetType().Name}"))
            .EnableSensitiveDataLogging()
            .LogTo(
                s =>
                {
                    if (LoggingEnabled)
                    {
                        Console.WriteLine(s);
                    }
                }, LogLevel.Information);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FeaturedPost>();

        // https://github.com/dotnet/efcore/issues/28671
        // modelBuilder.Entity<Author>().OwnsOne(e => e.Contact).OwnsOne(e => e.Address);
        modelBuilder.Entity<Author>().Ignore(e => e.Contact);
    }

    public async Task Seed()
    {
        var tagEntityFramework = new Tag("Entity Framework");
        var tagDotNet = new Tag(".NET");
        var tagDotNetMaui = new Tag(".NET MAUI");
        var tagAspDotNet = new Tag("ASP.NET");
        var tagAspDotNetCore = new Tag("ASP.NET Core");
        var tagDotNetCore = new Tag(".NET Core");
        var tagHacking = new Tag("Hacking");
        var tagLinux = new Tag("Linux");
        var tagSqlite = new Tag("SQLite");
        var tagVisualStudio = new Tag("Visual Studio");
        var tagGraphQl = new Tag("GraphQL");
        var tagCosmosDb = new Tag("CosmosDB");
        var tagBlazor = new Tag("Blazor");

        var maddy = new Author("Maddy Montaquila")
        {
            Contact = new() { Address = new("1 Main St", "Camberwick Green", "CW1 5ZH", "UK"), Phone = "01632 12345" }
        };
        var jeremy = new Author("Jeremy Likness")
        {
            Contact = new() { Address = new("2 Main St", "Camberwick Green", "CW1 5ZH", "UK"), Phone = "01632 12346" }
        };
        var dan = new Author("Daniel Roth")
        {
            Contact = new() { Address = new("3 Main St", "Camberwick Green", "CW1 5ZH", "UK"), Phone = "01632 12347" }
        };
        var arthur = new Author("Arthur Vickers")
        {
            Contact = new() { Address = new("15a Main St", "Camberwick Green", "CW1 5ZH", "UK"), Phone = "01632 12348" }
        };
        var brice = new Author("Brice Lambson")
        {
            Contact = new() { Address = new("4 Main St", "Camberwick Green", "CW1 5ZH", "UK"), Phone = "01632 12349" }
        };

        var blogs = new List<Blog>
        {
            new(".NET Blog")
            {
                Posts =
                {
                    new Post(
                        "Productivity comes to .NET MAUI in Visual Studio 2022",
                        "Visual Studio 2022 17.3 is now available and...",
                        new DateTime(2022, 8, 9)) { Tags = { tagDotNetMaui, tagDotNet }, Author = maddy, },
                    new Post(
                        "Announcing .NET 7 Preview 7", ".NET 7 Preview 7 is now available with improvements to System.LINQ, Unix...",
                        new DateTime(2022, 8, 9)) { Tags = { tagDotNet }, Author = jeremy, },
                    new Post(
                        "ASP.NET Core updates in .NET 7 Preview 7", ".NET 7 Preview 7 is now available! Check out what's new in...",
                        new DateTime(2022, 8, 9)) { Tags = { tagDotNet, tagAspDotNet, tagAspDotNetCore }, Author = dan, },
                    new FeaturedPost(
                        "Announcing Entity Framework 7 Preview 7: Interceptors!",
                        "Announcing EF7 Preview 7 with new and improved interceptors, and...",
                        new DateTime(2022, 8, 9),
                        "Loads of runnable code!") { Tags = { tagEntityFramework, tagDotNet, tagDotNetCore }, Author = arthur, }
                },
            },
            new("1unicorn2")
            {
                Posts =
                {
                    new Post(
                        "Hacking my Sixth Form College network in 1991",
                        "Back in 1991 I was a student at Franklin Sixth Form College...",
                        new DateTime(2020, 4, 10)) { Tags = { tagHacking }, Author = arthur, },
                    new FeaturedPost(
                        "All your versions are belong to us",
                        "Totally made up conversations about choosing Entity Framework version numbers...",
                        new DateTime(2020, 3, 26),
                        "Way funny!") { Tags = { tagEntityFramework }, Author = arthur, },
                    new Post(
                        "Moving to Linux", "A few weeks ago, I decided to move from Windows to Linux as...",
                        new DateTime(2020, 3, 7)) { Tags = { tagLinux }, Author = arthur, },
                    new Post(
                        "Welcome to One Unicorn 2.0!", "I created my first blog back in 2011..",
                        new DateTime(2020, 2, 29)) { Tags = { tagEntityFramework }, Author = arthur, }
                }
            },
            new("Brice's Blog")
            {
                Posts =
                {
                    new FeaturedPost(
                        "SQLite in Visual Studio 2022", "A couple of years ago, I was thinking of ways...",
                        new DateTime(2022, 7, 26), "Love for VS!") { Tags = { tagSqlite, tagVisualStudio }, Author = brice, },
                    new Post(
                        "On .NET - Entity Framework Migrations Explained",
                        "This week, @JamesMontemagno invited me onto the On .NET show...",
                        new DateTime(2022, 5, 4)) { Tags = { tagEntityFramework, tagDotNet }, Author = brice, },
                    new Post(
                        "Dear DBA: A silly idea", "We have fun on the Entity Framework team...",
                        new DateTime(2022, 3, 31)) { Tags = { tagEntityFramework }, Author = brice, },
                    new Post(
                        "Microsoft.Data.Sqlite 6", "It’s that time of year again. Microsoft.Data.Sqlite version...",
                        new DateTime(2021, 11, 8)) { Tags = { tagSqlite, tagDotNet }, Author = brice, }
                }
            },
            new("Developer for Life")
            {
                Posts =
                {
                    new Post(
                        "GraphQL for .NET Developers", "A comprehensive overview of GraphQL as...",
                        new DateTime(2021, 7, 1)) { Tags = { tagDotNet, tagGraphQl, tagAspDotNetCore }, Author = jeremy, },
                    new FeaturedPost(
                        "Azure Cosmos DB With EF Core on Blazor Server",
                        "Learn how to build Azure Cosmos DB apps using Entity Framework Core...",
                        new DateTime(2021, 5, 16),
                        "Blazor FTW!")
                    {
                        Tags =
                        {
                            tagDotNet,
                            tagEntityFramework,
                            tagAspDotNetCore,
                            tagCosmosDb,
                            tagBlazor
                        },
                        Author = jeremy,
                    },
                    new Post(
                        "Multi-tenancy with EF Core in Blazor Server Apps",
                        "Learn several ways to implement multi-tenant databases in Blazor Server apps...",
                        new DateTime(2021, 4, 29))
                    {
                        Tags = { tagDotNet, tagEntityFramework, tagAspDotNetCore, tagBlazor }, Author = jeremy,
                    },
                    new Post(
                        "An Easier Blazor Debounce", "Where I propose a simple method to debounce input without...",
                        new DateTime(2021, 4, 12)) { Tags = { tagDotNet, tagAspDotNetCore, tagBlazor }, Author = jeremy, }
                }
            }
        };

        await AddRangeAsync(blogs);
        await SaveChangesAsync();
    }
}

public enum MappingStrategy
{
    Tph,
    Tpt,
    Tpc,
}

public class TphBlogsContext : BlogsContext
{
    public TphBlogsContext()
        : base(useSqlite: false)
    {
    }

    public override MappingStrategy MappingStrategy => MappingStrategy.Tph;
}

public class TphSqliteBlogsContext : BlogsContext
{
    public TphSqliteBlogsContext()
        : base(useSqlite: true)
    {
    }

    public override MappingStrategy MappingStrategy => MappingStrategy.Tph;
}

public class TptBlogsContext : BlogsContext
{
    public TptBlogsContext()
        : base(useSqlite: false)
    {
    }

    public override MappingStrategy MappingStrategy => MappingStrategy.Tpt;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FeaturedPost>().ToTable("FeaturedPosts");

        base.OnModelCreating(modelBuilder);
    }
}

public class TpcBlogsContext : BlogsContext
{
    public TpcBlogsContext()
        : base(useSqlite: false)
    {
    }

    public override MappingStrategy MappingStrategy => MappingStrategy.Tpc;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>().UseTpcMappingStrategy();
        modelBuilder.Entity<FeaturedPost>().ToTable("FeaturedPosts");

        base.OnModelCreating(modelBuilder);
    }
}
