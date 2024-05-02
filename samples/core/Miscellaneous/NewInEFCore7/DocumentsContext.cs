using Microsoft.EntityFrameworkCore.Diagnostics;

namespace NewInEfCore7;

public abstract class Document
{
    protected Document(string title, int numberOfPages, DateTime publicationDate, byte[]? coverArt)
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
    public DateTime PublicationDate { get; set; }
    public byte[]? CoverArt { get; set; }

    public DateTime FirstRecordedOn { get; private set; }
    public DateTime RetrievedOn { get; private set; }
}

public class Book : Document
{
    public Book(string title, int numberOfPages, DateTime publicationDate, byte[]? coverArt)
        : base(title, numberOfPages, publicationDate, coverArt)
    {
    }

    public string? Isbn { get; set; }

    public List<Person> Authors { get; } = new();
}

public class Magazine : Document
{
    public Magazine(string title, int numberOfPages, DateTime publicationDate, byte[]? coverArt, int issueNumber)
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
    public virtual bool UseStoredProcedures => true;

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

        if (UseStoredProcedures)
        {
            #region PersonSprocs
            modelBuilder.Entity<Person>()
                .InsertUsingStoredProcedure(
                    "People_Insert",
                    storedProcedureBuilder =>
                    {
                        storedProcedureBuilder.HasParameter(a => a.Name);
                        storedProcedureBuilder.HasResultColumn(a => a.Id);
                    })
                .UpdateUsingStoredProcedure(
                    "People_Update",
                    storedProcedureBuilder =>
                    {
                        storedProcedureBuilder.HasOriginalValueParameter(person => person.Id);
                        storedProcedureBuilder.HasOriginalValueParameter(person => person.Name);
                        storedProcedureBuilder.HasParameter(person => person.Name);
                        storedProcedureBuilder.HasRowsAffectedResultColumn();
                    })
                .DeleteUsingStoredProcedure(
                    "People_Delete",
                    storedProcedureBuilder =>
                    {
                        storedProcedureBuilder.HasOriginalValueParameter(person => person.Id);
                        storedProcedureBuilder.HasOriginalValueParameter(person => person.Name);
                        storedProcedureBuilder.HasRowsAffectedResultColumn();
                    });
            #endregion
        }

        modelBuilder.Entity<Person>(
            entityTypeBuilder =>
            {
                entityTypeBuilder.OwnsOne(
                    author => author.Contact,
                    ownedNavigationBuilder =>
                    {
                        ownedNavigationBuilder.ToTable("Contacts");

                        if (UseStoredProcedures)
                        {
                            ownedNavigationBuilder
                                .InsertUsingStoredProcedure(
                                    storedProcedureBuilder =>
                                    {
                                        storedProcedureBuilder.HasParameter("PersonId");
                                        storedProcedureBuilder.HasParameter(contactDetails => contactDetails.Phone);
                                    })
                                .UpdateUsingStoredProcedure(
                                    storedProcedureBuilder =>
                                    {
                                        storedProcedureBuilder.HasOriginalValueParameter("PersonId");
                                        storedProcedureBuilder.HasParameter(contactDetails => contactDetails.Phone);
                                        storedProcedureBuilder.HasRowsAffectedResultColumn();
                                    })
                                .DeleteUsingStoredProcedure(
                                    storedProcedureBuilder =>
                                    {
                                        storedProcedureBuilder.HasOriginalValueParameter("PersonId");
                                        storedProcedureBuilder.HasRowsAffectedResultColumn();
                                    });
                        }

                        ownedNavigationBuilder.OwnsOne(
                            contactDetails => contactDetails.Address,
                            ownedOwnedNavigationBuilder =>
                            {
                                ownedOwnedNavigationBuilder.ToTable("Addresses");

                                if (UseStoredProcedures)
                                {
                                    ownedOwnedNavigationBuilder
                                        .InsertUsingStoredProcedure(
                                            storedProcedureBuilder =>
                                            {
                                                storedProcedureBuilder.HasParameter("ContactDetailsPersonId");
                                                storedProcedureBuilder.HasParameter(address => address.Street);
                                                storedProcedureBuilder.HasParameter(address => address.City);
                                                storedProcedureBuilder.HasParameter(address => address.Postcode);
                                                storedProcedureBuilder.HasParameter(address => address.Country);
                                            })
                                        .UpdateUsingStoredProcedure(
                                            storedProcedureBuilder =>
                                            {
                                                storedProcedureBuilder.HasOriginalValueParameter("ContactDetailsPersonId");
                                                storedProcedureBuilder.HasParameter(address => address.Street);
                                                storedProcedureBuilder.HasParameter(address => address.City);
                                                storedProcedureBuilder.HasParameter(address => address.Postcode);
                                                storedProcedureBuilder.HasParameter(address => address.Country);
                                                storedProcedureBuilder.HasRowsAffectedResultColumn();
                                            })
                                        .DeleteUsingStoredProcedure(
                                            storedProcedureBuilder =>
                                            {
                                                storedProcedureBuilder.HasOriginalValueParameter("ContactDetailsPersonId");
                                                storedProcedureBuilder.HasRowsAffectedResultColumn();
                                            });
                                }
                            });
                    });
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
            new Book("Extreme Programming Explained", 190, new DateTime(2000, 1, 1), null)
            {
                Isbn = "201-61641-6", Authors = { kentBeck }
            },
            new Book("Java Puzzlers", 283, new DateTime(2005, 1, 1), null)
            {
                Isbn = "0-321-33678-X", Authors = { joshuaBloch, nealGafter }
            },
            new Book("Effective Java", 252, new DateTime(2001, 1, 1), new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 })
            {
                Isbn = "0-201-31005-8", Authors = { joshuaBloch }
            },
            new Book("Test-Driven Development By Example", 220, new DateTime(2003, 1, 1), null)
            {
                Isbn = "0-321-14653-0", Authors = { kentBeck }
            },
            new Magazine("Amstrad Computer User", 95, new DateTime(1986, 1, 12), new byte[] { 1, 2, 3 }, 15)
            {
                CoverPrice = 0.95m, Editor = simonRockman
            },
            new Magazine("Amiga Computing", 90, new DateTime(1988, 5, 16), null, 1) { CoverPrice = 1.95m, Editor = simonRockman }
        };

        await AddRangeAsync(documents);
        await SaveChangesAsync();
    }
}

public class TphDocumentsContext : DocumentsContext
{
    public override MappingStrategy MappingStrategy => MappingStrategy.Tph;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>(
            entityTypeBuilder =>
            {
                if (UseStoredProcedures)
                {
                    entityTypeBuilder.InsertUsingStoredProcedure(
                            storedProcedureBuilder =>
                            {
                                storedProcedureBuilder.HasParameter("Discriminator");
                                storedProcedureBuilder.HasParameter(document => document.Title);
                                storedProcedureBuilder.HasParameter(document => document.NumberOfPages);
                                storedProcedureBuilder.HasParameter(document => document.PublicationDate);
                                storedProcedureBuilder.HasParameter(document => document.CoverArt);
                                storedProcedureBuilder.HasResultColumn(document => document.Id);
                                storedProcedureBuilder.HasParameter((Book document) => document.Isbn);
                                storedProcedureBuilder.HasParameter((Magazine document) => document.CoverPrice);
                                storedProcedureBuilder.HasParameter((Magazine document) => document.IssueNumber);
                                storedProcedureBuilder.HasParameter("EditorId");
                                storedProcedureBuilder.HasResultColumn(document => document.FirstRecordedOn);
                                storedProcedureBuilder.HasResultColumn(document => document.RetrievedOn);
                                storedProcedureBuilder.HasResultColumn(document => document.RowVersion);
                            })
                        .UpdateUsingStoredProcedure(
                            storedProcedureBuilder =>
                            {
                                storedProcedureBuilder.HasOriginalValueParameter(document => document.Id);
                                storedProcedureBuilder.HasOriginalValueParameter(document => document.RowVersion);
                                storedProcedureBuilder.HasParameter(document => document.Title);
                                storedProcedureBuilder.HasParameter(document => document.NumberOfPages);
                                storedProcedureBuilder.HasParameter(document => document.PublicationDate);
                                storedProcedureBuilder.HasParameter(document => document.CoverArt);
                                storedProcedureBuilder.HasParameter(document => ((Book)document).Isbn);
                                storedProcedureBuilder.HasParameter(document => ((Magazine)document).CoverPrice);
                                storedProcedureBuilder.HasParameter(document => ((Magazine)document).IssueNumber);
                                storedProcedureBuilder.HasParameter("EditorId");
                                storedProcedureBuilder.HasParameter(document => document.FirstRecordedOn);
                                storedProcedureBuilder.HasParameter(
                                    document => document.RetrievedOn, parameterBuilder => parameterBuilder.IsOutput());
                                storedProcedureBuilder.HasParameter(
                                    document => document.RowVersion, parameterBuilder => parameterBuilder.IsOutput());
                                storedProcedureBuilder.HasRowsAffectedResultColumn();
                            })
                        .DeleteUsingStoredProcedure(
                            storedProcedureBuilder =>
                            {
                                storedProcedureBuilder.HasOriginalValueParameter(document => document.Id);
                                storedProcedureBuilder.HasOriginalValueParameter(document => document.RowVersion);
                                storedProcedureBuilder.HasRowsAffectedResultColumn();
                            });
                }
            });

        if (UseStoredProcedures)
        {
            #region JoinSprocs
            modelBuilder.Entity<Book>(
                entityTypeBuilder =>
                {
                    entityTypeBuilder
                        .HasMany(document => document.Authors)
                        .WithMany(author => author.PublishedWorks)
                        .UsingEntity<Dictionary<string, object>>(
                            "BookPerson",
                            builder => builder.HasOne<Person>().WithMany().OnDelete(DeleteBehavior.Cascade),
                            builder => builder.HasOne<Book>().WithMany().OnDelete(DeleteBehavior.ClientCascade),
                            joinTypeBuilder =>
                            {
                                joinTypeBuilder
                                    .InsertUsingStoredProcedure(
                                        storedProcedureBuilder =>
                                        {
                                            storedProcedureBuilder.HasParameter("AuthorsId");
                                            storedProcedureBuilder.HasParameter("PublishedWorksId");
                                        })
                                    .DeleteUsingStoredProcedure(
                                        storedProcedureBuilder =>
                                        {
                                            storedProcedureBuilder.HasOriginalValueParameter("AuthorsId");
                                            storedProcedureBuilder.HasOriginalValueParameter("PublishedWorksId");
                                            storedProcedureBuilder.HasRowsAffectedResultColumn();
                                        });
                            });
                });
            #endregion
        }

        base.OnModelCreating(modelBuilder);
    }
}

public class TptDocumentsContext : DocumentsContext
{
    public override MappingStrategy MappingStrategy => MappingStrategy.Tpt;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>().UseTptMappingStrategy();

        modelBuilder.Entity<Document>(
            entityTypeBuilder =>
            {
                if (UseStoredProcedures)
                {
                    entityTypeBuilder
                        .InsertUsingStoredProcedure(
                            "Document_Insert_TPT",
                            storedProcedureBuilder =>
                            {
                                storedProcedureBuilder.HasParameter(document => document.Title);
                                storedProcedureBuilder.HasParameter(document => document.NumberOfPages);
                                storedProcedureBuilder.HasParameter(document => document.PublicationDate);
                                storedProcedureBuilder.HasParameter(document => document.CoverArt);
                                storedProcedureBuilder.HasResultColumn(document => document.Id);
                                storedProcedureBuilder.HasResultColumn(document => document.FirstRecordedOn);
                                storedProcedureBuilder.HasResultColumn(document => document.RetrievedOn);
                                storedProcedureBuilder.HasResultColumn(document => document.RowVersion);
                            })
                        .UpdateUsingStoredProcedure(
                            "Document_Update_TPT",
                            storedProcedureBuilder =>
                            {
                                storedProcedureBuilder.HasOriginalValueParameter(document => document.Id);
                                storedProcedureBuilder.HasOriginalValueParameter(document => document.RowVersion);
                                storedProcedureBuilder.HasParameter(document => document.Title);
                                storedProcedureBuilder.HasParameter(document => document.NumberOfPages);
                                storedProcedureBuilder.HasParameter(document => document.PublicationDate);
                                storedProcedureBuilder.HasParameter(document => document.CoverArt);
                                storedProcedureBuilder.HasParameter(document => document.FirstRecordedOn);
                                storedProcedureBuilder.HasParameter(
                                    magazine => magazine.RetrievedOn, parameterBuilder => parameterBuilder.IsOutput());
                                storedProcedureBuilder.HasParameter(
                                    magazine => magazine.RowVersion, parameterBuilder => parameterBuilder.IsOutput());
                                storedProcedureBuilder.HasRowsAffectedResultColumn();
                            })
                        .DeleteUsingStoredProcedure(
                            "Document_Delete_TPT",
                            storedProcedureBuilder =>
                            {
                                storedProcedureBuilder.HasOriginalValueParameter(document => document.Id);
                                storedProcedureBuilder.HasOriginalValueParameter(document => document.RowVersion);
                                storedProcedureBuilder.HasRowsAffectedResultColumn();
                            });
                }
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
                        builder => builder.HasOne<Book>().WithMany().OnDelete(DeleteBehavior.ClientCascade),
                        joinTypeBuilder =>
                        {
                            if (UseStoredProcedures)
                            {
                                joinTypeBuilder
                                    .InsertUsingStoredProcedure(
                                        storedProcedureBuilder =>
                                        {
                                            storedProcedureBuilder.HasParameter("AuthorsId");
                                            storedProcedureBuilder.HasParameter("PublishedWorksId");
                                        })
                                    .DeleteUsingStoredProcedure(
                                        storedProcedureBuilder =>
                                        {
                                            storedProcedureBuilder.HasOriginalValueParameter("AuthorsId");
                                            storedProcedureBuilder.HasOriginalValueParameter("PublishedWorksId");
                                            storedProcedureBuilder.HasRowsAffectedResultColumn();
                                        });
                            }
                        });

                if (UseStoredProcedures)
                {
                    entityTypeBuilder
                        .InsertUsingStoredProcedure(
                            "Book_Insert_TPT",
                            storedProcedureBuilder =>
                            {
                                storedProcedureBuilder.HasParameter(book => book.Id);
                                storedProcedureBuilder.HasParameter(book => book.Isbn);
                            })
                        .UpdateUsingStoredProcedure(
                            "Book_Update_TPT",
                            storedProcedureBuilder =>
                            {
                                storedProcedureBuilder.HasOriginalValueParameter(book => book.Id);
                                storedProcedureBuilder.HasParameter(book => book.Isbn);
                                storedProcedureBuilder.HasRowsAffectedResultColumn();
                            })
                        .DeleteUsingStoredProcedure(
                            "Book_Delete_TPT",
                            storedProcedureBuilder =>
                            {
                                storedProcedureBuilder.HasOriginalValueParameter(book => book.Id);
                                storedProcedureBuilder.HasRowsAffectedResultColumn();
                            });
                }
            });

        modelBuilder.Entity<Magazine>(
            entityTypeBuilder =>
            {
                if (UseStoredProcedures)
                {
                    entityTypeBuilder
                        .InsertUsingStoredProcedure(
                            "Magazine_Insert_TPT",
                            storedProcedureBuilder =>
                            {
                                storedProcedureBuilder.HasParameter(magazine => magazine.Id);
                                storedProcedureBuilder.HasParameter(magazine => magazine.CoverPrice);
                                storedProcedureBuilder.HasParameter(magazine => magazine.IssueNumber);
                                storedProcedureBuilder.HasParameter("EditorId");
                            })
                        .UpdateUsingStoredProcedure(
                            "Magazine_Update_TPT",
                            storedProcedureBuilder =>
                            {
                                storedProcedureBuilder.HasOriginalValueParameter(magazine => magazine.Id);
                                storedProcedureBuilder.HasParameter(magazine => magazine.CoverPrice);
                                storedProcedureBuilder.HasParameter(magazine => magazine.IssueNumber);
                                storedProcedureBuilder.HasParameter("EditorId");
                                storedProcedureBuilder.HasRowsAffectedResultColumn();
                            })
                        .DeleteUsingStoredProcedure(
                            "Magazine_Delete_TPT",
                            storedProcedureBuilder =>
                            {
                                storedProcedureBuilder.HasOriginalValueParameter(magazine => magazine.Id);
                                storedProcedureBuilder.HasRowsAffectedResultColumn();
                            });
                }
            });

        base.OnModelCreating(modelBuilder);
    }
}

public class TpcDocumentsContext : DocumentsContext
{
    public override MappingStrategy MappingStrategy => MappingStrategy.Tpc;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>().UseTpcMappingStrategy();

        modelBuilder.Entity<Book>(
            entityTypeBuilder =>
            {
                entityTypeBuilder
                    .HasMany(document => document.Authors)
                    .WithMany(author => author.PublishedWorks)
                    .UsingEntity<Dictionary<string, object>>(
                        "BookPerson",
                        builder => builder.HasOne<Person>().WithMany().OnDelete(DeleteBehavior.Cascade),
                        builder => builder.HasOne<Book>().WithMany().OnDelete(DeleteBehavior.ClientCascade),
                        joinTypeBuilder =>
                        {
                            if (UseStoredProcedures)
                            {
                                joinTypeBuilder
                                    .InsertUsingStoredProcedure(
                                        storedProcedureBuilder =>
                                        {
                                            storedProcedureBuilder.HasParameter("AuthorsId");
                                            storedProcedureBuilder.HasParameter("PublishedWorksId");
                                        })
                                    .DeleteUsingStoredProcedure(
                                        storedProcedureBuilder =>
                                        {
                                            storedProcedureBuilder.HasOriginalValueParameter("AuthorsId");
                                            storedProcedureBuilder.HasOriginalValueParameter("PublishedWorksId");
                                            storedProcedureBuilder.HasRowsAffectedResultColumn();
                                        });
                            }
                        });

                if (UseStoredProcedures)
                {
                    entityTypeBuilder.InsertUsingStoredProcedure(
                            "Book_Insert_TPC",
                            storedProcedureBuilder =>
                            {
                                storedProcedureBuilder.HasParameter(book => book.Title);
                                storedProcedureBuilder.HasParameter(book => book.NumberOfPages);
                                storedProcedureBuilder.HasParameter(book => book.PublicationDate);
                                storedProcedureBuilder.HasParameter(book => book.CoverArt);
                                storedProcedureBuilder.HasParameter(book => book.Isbn);
                                storedProcedureBuilder.HasResultColumn(book => book.Id);
                                storedProcedureBuilder.HasResultColumn(book => book.FirstRecordedOn);
                                storedProcedureBuilder.HasResultColumn(book => book.RetrievedOn);
                                storedProcedureBuilder.HasResultColumn(book => book.RowVersion);
                            })
                        .UpdateUsingStoredProcedure(
                            "Book_Update_TPC",
                            storedProcedureBuilder =>
                            {
                                storedProcedureBuilder.HasOriginalValueParameter(book => book.Id);
                                storedProcedureBuilder.HasOriginalValueParameter(magazine => magazine.RowVersion);
                                storedProcedureBuilder.HasParameter(book => book.Title);
                                storedProcedureBuilder.HasParameter(book => book.NumberOfPages);
                                storedProcedureBuilder.HasParameter(book => book.PublicationDate);
                                storedProcedureBuilder.HasParameter(book => book.CoverArt);
                                storedProcedureBuilder.HasParameter(book => book.FirstRecordedOn);
                                storedProcedureBuilder.HasParameter(book => book.Isbn);
                                storedProcedureBuilder.HasParameter(
                                    magazine => magazine.RetrievedOn, parameterBuilder => parameterBuilder.IsOutput());
                                storedProcedureBuilder.HasParameter(
                                    magazine => magazine.RowVersion, parameterBuilder => parameterBuilder.IsOutput());
                                storedProcedureBuilder.HasRowsAffectedResultColumn();
                            })
                        .DeleteUsingStoredProcedure(
                            "Book_Delete_TPC",
                            storedProcedureBuilder =>
                            {
                                storedProcedureBuilder.HasOriginalValueParameter(book => book.Id);
                                storedProcedureBuilder.HasOriginalValueParameter(book => book.RowVersion);
                                storedProcedureBuilder.HasRowsAffectedResultColumn();
                            });
                }
            });

        modelBuilder.Entity<Magazine>(
            entityTypeBuilder =>
            {
                if (UseStoredProcedures)
                {
                    entityTypeBuilder
                        .InsertUsingStoredProcedure(
                            "Magazine_Insert_TPC",
                            storedProcedureBuilder =>
                            {
                                storedProcedureBuilder.HasParameter(magazine => magazine.Title);
                                storedProcedureBuilder.HasParameter(magazine => magazine.NumberOfPages);
                                storedProcedureBuilder.HasParameter(magazine => magazine.PublicationDate);
                                storedProcedureBuilder.HasParameter(magazine => magazine.CoverArt);
                                storedProcedureBuilder.HasResultColumn(magazine => magazine.Id);
                                storedProcedureBuilder.HasParameter(magazine => magazine.CoverPrice);
                                storedProcedureBuilder.HasParameter(magazine => magazine.IssueNumber);
                                storedProcedureBuilder.HasParameter("EditorId");
                                storedProcedureBuilder.HasResultColumn(magazine => magazine.FirstRecordedOn);
                                storedProcedureBuilder.HasResultColumn(magazine => magazine.RetrievedOn);
                                storedProcedureBuilder.HasResultColumn(magazine => magazine.RowVersion);
                            })
                        .UpdateUsingStoredProcedure(
                            "Magazine_Update_TPC",
                            storedProcedureBuilder =>
                            {
                                storedProcedureBuilder.HasOriginalValueParameter(magazine => magazine.Id);
                                storedProcedureBuilder.HasOriginalValueParameter(magazine => magazine.RowVersion);
                                storedProcedureBuilder.HasParameter(magazine => magazine.Title);
                                storedProcedureBuilder.HasParameter(magazine => magazine.NumberOfPages);
                                storedProcedureBuilder.HasParameter(magazine => magazine.PublicationDate);
                                storedProcedureBuilder.HasParameter(magazine => magazine.CoverArt);
                                storedProcedureBuilder.HasParameter(magazine => magazine.CoverPrice);
                                storedProcedureBuilder.HasParameter(magazine => magazine.IssueNumber);
                                storedProcedureBuilder.HasParameter("EditorId");
                                storedProcedureBuilder.HasParameter(magazine => magazine.FirstRecordedOn);
                                storedProcedureBuilder.HasParameter(
                                    magazine => magazine.RetrievedOn, parameterBuilder => parameterBuilder.IsOutput());
                                storedProcedureBuilder.HasParameter(
                                    magazine => magazine.RowVersion, parameterBuilder => parameterBuilder.IsOutput());
                                storedProcedureBuilder.HasRowsAffectedResultColumn();
                            })
                        .DeleteUsingStoredProcedure(
                            "Magazine_Delete_TPC",
                            storedProcedureBuilder =>
                            {
                                storedProcedureBuilder.HasOriginalValueParameter(magazine => magazine.Id);
                                storedProcedureBuilder.HasOriginalValueParameter(magazine => magazine.RowVersion);
                                storedProcedureBuilder.HasRowsAffectedResultColumn();
                            });
                }
            });

        base.OnModelCreating(modelBuilder);
    }
}
