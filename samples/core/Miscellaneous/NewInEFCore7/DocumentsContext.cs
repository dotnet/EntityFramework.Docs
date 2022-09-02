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
        => optionsBuilder.UseSqlServer(@$"Server=(localdb)\mssqllocaldb;Database={GetType().Name}")
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
        modelBuilder.Entity<Document>(
            entityTypeBuilder =>
            {
                entityTypeBuilder.Property(document => document.FirstRecordedOn).HasDefaultValueSql("getutcdate()");
                entityTypeBuilder.Property(document => document.RetrievedOn).HasComputedColumnSql("getutcdate()");
            });

        modelBuilder.Entity<Person>(
            entityTypeBuilder =>
            {
                if (UseStoredProcedures)
                {
                    entityTypeBuilder.InsertUsingStoredProcedure(
                        "Person_Insert", storedProcedureBuilder =>
                        {
                            storedProcedureBuilder.HasParameter(a => a.Name);
                            storedProcedureBuilder.HasResultColumn(a => a.Id);
                        });

                    entityTypeBuilder.UpdateUsingStoredProcedure(
                        "Person_Update",
                        storedProcedureBuilder =>
                        {
                            storedProcedureBuilder.HasParameter(person => person.Id);
                            storedProcedureBuilder.HasOriginalValueParameter(
                                person => person.Name, parameterBuilder => parameterBuilder.HasName("Name_Original"));
                            storedProcedureBuilder.HasParameter(person => person.Name);
                            storedProcedureBuilder.HasRowsAffectedResultColumn();
                        });

                    entityTypeBuilder.DeleteUsingStoredProcedure(
                        "Person_Delete",
                        storedProcedureBuilder =>
                        {
                            storedProcedureBuilder.HasParameter(person => person.Id);
                            storedProcedureBuilder.HasOriginalValueParameter(person => person.Name);
                            storedProcedureBuilder.HasRowsAffectedResultColumn();
                        });
                }

                entityTypeBuilder.OwnsOne(
                    author => author.Contact,
                    ownedNavigationBuilder =>
                    {
                        ownedNavigationBuilder.ToTable("Contacts");

                        if (UseStoredProcedures)
                        {
                            ownedNavigationBuilder.InsertUsingStoredProcedure(
                                "Contacts_Insert", storedProcedureBuilder =>
                                {
                                    storedProcedureBuilder.HasParameter("PersonId");
                                    storedProcedureBuilder.HasParameter(contactDetails => contactDetails.Phone);
                                });

                            ownedNavigationBuilder.UpdateUsingStoredProcedure(
                                "Contacts_Update",
                                storedProcedureBuilder =>
                                {
                                    storedProcedureBuilder.HasParameter("PersonId");
                                    storedProcedureBuilder.HasParameter(contactDetails => contactDetails.Phone);
                                    storedProcedureBuilder.HasRowsAffectedResultColumn();
                                });

                            ownedNavigationBuilder.DeleteUsingStoredProcedure(
                                "Contacts_Delete", storedProcedureBuilder =>
                                {
                                    storedProcedureBuilder.HasParameter("PersonId");
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
                                    ownedOwnedNavigationBuilder.InsertUsingStoredProcedure(
                                        "Addresses_Insert", storedProcedureBuilder =>
                                        {
                                            storedProcedureBuilder.HasParameter("ContactDetailsPersonId");
                                            storedProcedureBuilder.HasParameter(address => address.Street);
                                            storedProcedureBuilder.HasParameter(address => address.City);
                                            storedProcedureBuilder.HasParameter(address => address.Postcode);
                                            storedProcedureBuilder.HasParameter(address => address.Country);
                                        });

                                    ownedOwnedNavigationBuilder.UpdateUsingStoredProcedure(
                                        "Addresses_Update",
                                        storedProcedureBuilder =>
                                        {
                                            storedProcedureBuilder.HasParameter("ContactDetailsPersonId");
                                            storedProcedureBuilder.HasParameter(address => address.Street);
                                            storedProcedureBuilder.HasParameter(address => address.City);
                                            storedProcedureBuilder.HasParameter(address => address.Postcode);
                                            storedProcedureBuilder.HasParameter(address => address.Country);
                                            storedProcedureBuilder.HasRowsAffectedResultColumn();
                                        });

                                    ownedOwnedNavigationBuilder.DeleteUsingStoredProcedure(
                                        "Addresses_Delete", storedProcedureBuilder =>
                                        {
                                            storedProcedureBuilder.HasParameter("ContactDetailsPersonId");
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
                        "Document_Insert",
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
                        });

                    entityTypeBuilder.UpdateUsingStoredProcedure(
                        "Document_Update",
                        storedProcedureBuilder =>
                        {
                            storedProcedureBuilder.HasParameter(document => document.Id);
                            storedProcedureBuilder.HasParameter("Discriminator");
                            storedProcedureBuilder.HasOriginalValueParameter(
                                document => document.RowVersion,
                                parameterBuilder => parameterBuilder.HasName("RowVersion_Original"));
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
                        });

                    entityTypeBuilder.DeleteUsingStoredProcedure(
                        "Document_Delete",
                        storedProcedureBuilder =>
                        {
                            storedProcedureBuilder.HasParameter(document => document.Id);
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
                                joinTypeBuilder.InsertUsingStoredProcedure(
                                    "BookPerson_Insert",
                                    storedProcedureBuilder =>
                                    {
                                        storedProcedureBuilder.HasParameter("AuthorsId");
                                        storedProcedureBuilder.HasParameter("PublishedWorksId");
                                    });

                                joinTypeBuilder.DeleteUsingStoredProcedure(
                                    "BookPerson_Delete",
                                    storedProcedureBuilder =>
                                    {
                                        storedProcedureBuilder.HasParameter("AuthorsId");
                                        storedProcedureBuilder.HasParameter("PublishedWorksId");
                                        storedProcedureBuilder.HasRowsAffectedResultColumn();
                                    });
                            }
                        });
            });

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
                    entityTypeBuilder.InsertUsingStoredProcedure(
                        "Document_Insert",
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
                        });

                    entityTypeBuilder.UpdateUsingStoredProcedure(
                        "Document_Update",
                        storedProcedureBuilder =>
                        {
                            storedProcedureBuilder.HasParameter(document => document.Id);
                            storedProcedureBuilder.HasOriginalValueParameter(
                                document => document.RowVersion,
                                parameterBuilder => parameterBuilder.HasName("RowVersion_Original"));
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
                        });

                    entityTypeBuilder.DeleteUsingStoredProcedure(
                        "Document_Delete",
                        storedProcedureBuilder =>
                        {
                            storedProcedureBuilder.HasParameter(document => document.Id);
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
                                joinTypeBuilder.InsertUsingStoredProcedure(
                                    "BookPerson_Insert",
                                    storedProcedureBuilder =>
                                    {
                                        storedProcedureBuilder.HasParameter("AuthorsId");
                                        storedProcedureBuilder.HasParameter("PublishedWorksId");
                                    });

                                joinTypeBuilder.DeleteUsingStoredProcedure(
                                    "BookPerson_Delete",
                                    storedProcedureBuilder =>
                                    {
                                        storedProcedureBuilder.HasParameter("AuthorsId");
                                        storedProcedureBuilder.HasParameter("PublishedWorksId");
                                        storedProcedureBuilder.HasRowsAffectedResultColumn();
                                    });
                            }
                        });

                if (UseStoredProcedures)
                {
                    entityTypeBuilder.InsertUsingStoredProcedure(
                        "Book_Insert",
                        storedProcedureBuilder =>
                        {
                            storedProcedureBuilder.HasParameter(book => book.Id);
                            storedProcedureBuilder.HasParameter(book => book.Isbn);
                        });

                    entityTypeBuilder.UpdateUsingStoredProcedure(
                        "Book_Update",
                        storedProcedureBuilder =>
                        {
                            storedProcedureBuilder.HasParameter(book => book.Id);
                            storedProcedureBuilder.HasParameter(book => book.Isbn);
                            storedProcedureBuilder.HasRowsAffectedResultColumn();
                        });

                    entityTypeBuilder.DeleteUsingStoredProcedure(
                        "Book_Delete",
                        storedProcedureBuilder =>
                        {
                            storedProcedureBuilder.HasParameter(book => book.Id);
                            storedProcedureBuilder.HasRowsAffectedResultColumn();
                        });
                }
            });

        modelBuilder.Entity<Magazine>(
            entityTypeBuilder =>
            {
                if (UseStoredProcedures)
                {
                    entityTypeBuilder.InsertUsingStoredProcedure(
                        "Magazine_Insert",
                        storedProcedureBuilder =>
                        {
                            storedProcedureBuilder.HasParameter(magazine => magazine.Id);
                            storedProcedureBuilder.HasParameter(magazine => magazine.CoverPrice);
                            storedProcedureBuilder.HasParameter(magazine => magazine.IssueNumber);
                            storedProcedureBuilder.HasParameter("EditorId");
                        });

                    entityTypeBuilder.UpdateUsingStoredProcedure(
                        "Magazine_Update",
                        storedProcedureBuilder =>
                        {
                            storedProcedureBuilder.HasParameter(magazine => magazine.Id);
                            storedProcedureBuilder.HasParameter(magazine => magazine.CoverPrice);
                            storedProcedureBuilder.HasParameter(magazine => magazine.IssueNumber);
                            storedProcedureBuilder.HasParameter("EditorId");
                            storedProcedureBuilder.HasRowsAffectedResultColumn();
                        });

                    entityTypeBuilder.DeleteUsingStoredProcedure(
                        "Magazine_Delete",
                        storedProcedureBuilder =>
                        {
                            storedProcedureBuilder.HasParameter(magazine => magazine.Id);
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
                                joinTypeBuilder.InsertUsingStoredProcedure(
                                    "BookPerson_Insert",
                                    storedProcedureBuilder =>
                                    {
                                        storedProcedureBuilder.HasParameter("AuthorsId");
                                        storedProcedureBuilder.HasParameter("PublishedWorksId");
                                    });

                                joinTypeBuilder.DeleteUsingStoredProcedure(
                                    "BookPerson_Delete",
                                    storedProcedureBuilder =>
                                    {
                                        storedProcedureBuilder.HasParameter("AuthorsId");
                                        storedProcedureBuilder.HasParameter("PublishedWorksId");
                                        storedProcedureBuilder.HasRowsAffectedResultColumn();
                                    });
                            }
                        });

                if (UseStoredProcedures)
                {
                    entityTypeBuilder.InsertUsingStoredProcedure(
                        "Book_Insert",
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
                        });

                    entityTypeBuilder.UpdateUsingStoredProcedure(
                        "Book_Update",
                        storedProcedureBuilder =>
                        {
                            storedProcedureBuilder.HasParameter(book => book.Id);
                            storedProcedureBuilder.HasOriginalValueParameter(
                                magazine => magazine.RowVersion,
                                parameterBuilder => parameterBuilder.HasName("RowVersion_Original"));
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
                        });

                    entityTypeBuilder.DeleteUsingStoredProcedure(
                        "Book_Delete",
                        storedProcedureBuilder =>
                        {
                            storedProcedureBuilder.HasParameter(book => book.Id);
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
                    entityTypeBuilder.InsertUsingStoredProcedure(
                        "Magazine_Insert",
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
                        });

                    entityTypeBuilder.UpdateUsingStoredProcedure(
                        "Magazine_Update",
                        storedProcedureBuilder =>
                        {
                            storedProcedureBuilder.HasParameter(magazine => magazine.Id);
                            storedProcedureBuilder.HasOriginalValueParameter(
                                magazine => magazine.RowVersion,
                                parameterBuilder => parameterBuilder.HasName("RowVersion_Original"));
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
                        });

                    entityTypeBuilder.DeleteUsingStoredProcedure(
                        "Magazine_Delete",
                        storedProcedureBuilder =>
                        {
                            storedProcedureBuilder.HasParameter(magazine => magazine.Id);
                            storedProcedureBuilder.HasOriginalValueParameter(magazine => magazine.RowVersion);
                            storedProcedureBuilder.HasRowsAffectedResultColumn();
                        });
                }
            });

        base.OnModelCreating(modelBuilder);
    }
}
