using Microsoft.EntityFrameworkCore.Diagnostics;

namespace NewInEfCore8;

public abstract class Document
{
    protected Document(string title, int numberOfPages, DateOnly publicationDate, byte[]? coverArt)
    {
        Title = title;
        NumberOfPages = numberOfPages;
        PublicationDate = publicationDate;
        CoverArt = coverArt;
    }

    public int Id { get; private set; }

    [Timestamp]
    public byte[]? RowVersion { get; set; }

    public string Title { get; set; }
    public int NumberOfPages { get; set; }
    public DateOnly PublicationDate { get; set; }
    public byte[]? CoverArt { get; set; }

    public DateOnly FirstRecordedOn { get; private set; }
    public DateOnly RetrievedOn { get; private set; }
}

public class Book : Document
{
    public Book(string title, int numberOfPages, DateOnly publicationDate, byte[]? coverArt)
        : base(title, numberOfPages, publicationDate, coverArt)
    {
    }

    public string? Isbn { get; set; }

    public List<Person> Authors { get; } = new();
}

public class Magazine : Document
{
    public Magazine(string title, int numberOfPages, DateOnly publicationDate, byte[]? coverArt, int issueNumber)
        : base(title, numberOfPages, publicationDate, coverArt)
    {
        IssueNumber = issueNumber;
    }

    public int IssueNumber { get; set; }
    public decimal? CoverPrice { get; set; }
    public Person Editor { get; set; } = null!;
}

public class Person
{
    public Person(string name)
    {
        Name = name;
    }

    public int Id { get; private set; }

    [ConcurrencyCheck]
    public string Name { get; set; }

    public ContactDetails Contact { get; set; } = null!;

    public List<Book> PublishedWorks { get; } = new();
    public List<Magazine> Edited { get; } = new();
}

public abstract class DocumentsContext : DbContext
{
    public bool LoggingEnabled { get; set; }
    public abstract MappingStrategy MappingStrategy { get; }

    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Magazine> Magazines => Set<Magazine>();
    public DbSet<Person> People => Set<Person>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(@$"Server=(localdb)\mssqllocaldb;Database={GetType().Name};ConnectRetryCount=0")
            .EnableSensitiveDataLogging()
            .LogTo(
                s =>
                {
                    if (LoggingEnabled)
                    {
                        Console.WriteLine(s);
                    }
                }, new List<EventId> { RelationalEventId.CommandExecuted });

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>(
            entityTypeBuilder =>
            {
                entityTypeBuilder.Property(document => document.FirstRecordedOn).HasDefaultValueSql("getutcdate()");
                entityTypeBuilder.Property(document => document.RetrievedOn).HasComputedColumnSql("getutcdate()");
            });

        modelBuilder.Entity<Person>(
            entityTypeBuilder =>
            {
                entityTypeBuilder.OwnsOne(
                    author => author.Contact,
                    ownedNavigationBuilder =>
                    {
                        ownedNavigationBuilder.ToTable("Contacts");
                        ownedNavigationBuilder.OwnsOne(
                            contactDetails => contactDetails.Address,
                            ownedOwnedNavigationBuilder =>
                            {
                                ownedOwnedNavigationBuilder.ToTable("Addresses");
                            });
                    });
            });

        modelBuilder.Entity<Book>(
            entityTypeBuilder =>
            {
                entityTypeBuilder
                    .HasMany(document => document.Authors)
                    .WithMany(author => author.PublishedWorks)
                    .UsingEntity<Dictionary<string, object>>(
                        "BookPerson",
                        builder => builder.HasOne<Person>().WithMany().OnDelete(DeleteBehavior.Cascade),
                        builder => builder.HasOne<Book>().WithMany().OnDelete(DeleteBehavior.ClientCascade));
            });
    }

    public async Task Seed()
    {
        var kentBeck = new Person("Kent Beck")
        {
            Contact = new() { Address = new Address("1 Smalltalk Ave", "Camberwick Green", "CW1 5ZH", "UK"), Phone = "01632 12346" }
        };

        var joshuaBloch = new Person("Joshua Bloch")
        {
            Contact = new() { Address = new Address("1 AFS Walk", "Chigley", "CW1 5ZH", "UK"), Phone = "01632 12347" }
        };

        var nealGafter = new Person("Neal Gafter")
        {
            Contact = new() { Address = new Address("1 Merlin Closure", "Chigley", "CW1 5ZH", "UK"), Phone = "01632 12348" }
        };

        var simonRockman = new Person("Simon Rockman")
        {
            Contact = new() { Address = new Address("1 Copper Run", "Camberwick Green", "CW1 5ZH", "UK"), Phone = "01632 12349" }
        };

        var documents = new List<Document>
        {
            new Book("Extreme Programming Explained", 190, new DateOnly(2000, 1, 1), null)
            {
                Isbn = "201-61641-6", Authors = { kentBeck }
            },
            new Book("Java Puzzlers", 283, new DateOnly(2005, 1, 1), null)
            {
                Isbn = "0-321-33678-X", Authors = { joshuaBloch, nealGafter }
            },
            new Book("Effective Java", 252, new DateOnly(2001, 1, 1), new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 })
            {
                Isbn = "0-201-31005-8", Authors = { joshuaBloch }
            },
            new Book("Test-Driven Development By Example", 220, new DateOnly(2003, 1, 1), null)
            {
                Isbn = "0-321-14653-0", Authors = { kentBeck }
            },
            new Magazine("Amstrad Computer User", 95, new DateOnly(1986, 1, 12), new byte[] { 1, 2, 3 }, 15)
            {
                CoverPrice = 0.95m, Editor = simonRockman
            },
            new Magazine("Amiga Computing", 90, new DateOnly(1988, 5, 16), null, 1) { CoverPrice = 1.95m, Editor = simonRockman }
        };

        await AddRangeAsync(documents);
        await SaveChangesAsync();
    }
}

public class TphDocumentsContext : DocumentsContext
{
    public override MappingStrategy MappingStrategy => MappingStrategy.Tph;
}

public class TptDocumentsContext : DocumentsContext
{
    public override MappingStrategy MappingStrategy => MappingStrategy.Tpt;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>().UseTptMappingStrategy();

        base.OnModelCreating(modelBuilder);
    }
}

public class TpcDocumentsContext : DocumentsContext
{
    public override MappingStrategy MappingStrategy => MappingStrategy.Tpc;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>().UseTpcMappingStrategy();

        base.OnModelCreating(modelBuilder);
    }
}
