namespace NewInEfCore9;

public class Writer
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    public List<Book> BooksWritten { get; set; } = null!;
}

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public DateTime PublishDate { get; set; }
    public Series? PartOf { get; set; }
}

public class Series
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public List<Book> Books { get; set; } = null!;
    public bool Completed { get; set; }
}

public class DataSeedingContext : DbContext
{
    public DbSet<Writer> Writers => Set<Writer>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Series> Series => Set<Series>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Writer>().Property(x => x.Id).ValueGeneratedNever();
        modelBuilder.Entity<Book>().Property(x => x.Id).ValueGeneratedNever();
        modelBuilder.Entity<Series>().Property(x => x.Id).ValueGeneratedNever();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        #region DataSeedingConfiguration
        optionsBuilder.UseSqlServer(@$"Server=(localdb)\mssqllocaldb;Database=DataSeedingContext;ConnectRetryCount=0")
            .UseAsyncSeeding(async (ctx, b, ct) =>
            {
                if (!ctx.Set<Book>().Any()
                    && !ctx.Set<Writer>().Any()
                    && !ctx.Set<Series>().Any())
                {
                    var firstLawSeries = new Series { Id = 1, Name = "First Law", Completed = true };
                    var joeAbercrombie = new Writer
                    {
                        Id = 1,
                        FirstName = "Joe",
                        LastName = "Abercrombie",
                        BooksWritten =
                        [
                            new Book { Id = 1, Title = "The Blade Itself", PartOf = firstLawSeries, PublishDate = new DateTime(2006, 5, 4) },
                            new Book { Id = 2, Title = "Before They Are ...", PartOf = firstLawSeries, PublishDate = new DateTime(2007, 3, 1) },
                            new Book { Id = 3, Title = "Last Argument of Kings", PartOf = firstLawSeries, PublishDate = new DateTime(2008, 3, 20)},
                        ]
                    };

                    var stormlightArchive = new Series { Id = 2, Name = "The Stormlight Archive", Completed = false };
                    var brandonSanderson = new Writer
                    {
                        Id = 2,
                        FirstName = "Brandon",
                        LastName = "Sanderson",
                        BooksWritten =
                        [
                            new Book { Id = 4, Title = "Elantris", PublishDate = new DateTime(2005, 4, 21) },
                            new Book { Id = 5, Title = "The Way of Kings", PartOf = stormlightArchive, PublishDate = new DateTime(2010, 8, 31) },
                            new Book { Id = 6, Title = "Words of Radiance", PartOf = stormlightArchive, PublishDate = new DateTime(2014, 3, 1) },
                            new Book { Id = 7, Title = "Oathbringer", PartOf = stormlightArchive, PublishDate = new DateTime(2017, 11, 14) },
                            new Book { Id = 8, Title = "Rhythm of War", PartOf = stormlightArchive, PublishDate = new DateTime(2020, 11, 17) },
                        ]
                    };

                    var duneSeries = new Series { Id = 3, Name = "Dune", Completed = true };
                    var frankHerbert = new Writer
                    {
                        Id = 3,
                        FirstName = "Frank",
                        LastName = "Herbert",
                        BooksWritten =
                        [
                            new Book { Id = 9, Title = "Dune", PartOf = duneSeries, PublishDate = new DateTime(1965, 8, 1) },
                            new Book { Id = 10, Title = "Dune Messiah", PartOf = duneSeries, PublishDate = new DateTime(1969, 6, 1) },
                            new Book { Id = 11, Title = "Children of Dune", PartOf = duneSeries, PublishDate = new DateTime(1976, 4, 1) },
                            new Book { Id = 12, Title = "Heretics of Dune", PartOf = duneSeries, PublishDate = new DateTime(1984, 1, 1) },
                            new Book { Id = 13, Title = "Chapterhouse: Dune", PartOf = duneSeries, PublishDate = new DateTime(1985, 4, 1) },
                        ]
                    };

                    ctx.Set<Writer>().AddRange(joeAbercrombie, frankHerbert);
                    await ctx.SaveChangesAsync(ct);
                }
            });
        #endregion
        optionsBuilder.EnableSensitiveDataLogging()
            .LogTo(Console.WriteLine, LogLevel.Information);
}
