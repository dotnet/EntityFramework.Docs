using Microsoft.EntityFrameworkCore;

namespace EFModeling.DataSeeding;

public class ManagingDataContext : DbContext
{
    public DbSet<Country> Countries { get; set; }
    public DbSet<City> Cites { get; set; }
    public DbSet<Language> Languages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFManagingData;Trusted_Connection=True;ConnectRetryCount=0");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region CountrySeed
        modelBuilder.Entity<Country>(b =>
        {
            b.Property(x => x.Name).IsRequired();
            b.HasData(
                new Country { CountryId = 1, Name = "USA" },
                new Country { CountryId = 2, Name = "Canada" },
                new Country { CountryId = 3, Name = "Mexico" });
        });
        #endregion

        #region CitySeed
        modelBuilder.Entity<City>().HasData(
            new City { Id = 1, Name = "Seattle", LocatedInId = 1 },
            new City { Id = 2, Name = "Vancouver", LocatedInId = 2 },
            new City { Id = 3, Name = "Mexico City", LocatedInId = 3 },
            new City { Id = 4, Name = "Puebla", LocatedInId = 3 });
        #endregion

        #region LanguageSeed
        modelBuilder.Entity<Language>(b =>
        {
            b.HasData(
                new Language { Id = 1, Name = "English" },
                new Language { Id = 2, Name = "French" },
                new Language { Id = 3, Name = "Spanish" });

            b.HasMany(x => x.UsedIn)
                .WithMany(x => x.OfficialLanguages)
                .UsingEntity(
                    "LanguageCountry",
                    r => r.HasOne(typeof(Country)).WithMany().HasForeignKey("CountryId").HasPrincipalKey(nameof(Country.CountryId)),
                    l => l.HasOne(typeof(Language)).WithMany().HasForeignKey("LanguageId").HasPrincipalKey(nameof(Language.Id)),
                    je =>
                    {
                        je.HasKey("LanguageId", "CountryId");
                        je.HasData(
                            new { LanguageId = 1, CountryId = 2 },
                            new { LanguageId = 2, CountryId = 2 },
                            new { LanguageId = 3, CountryId = 3 });
                    });
        });
        #endregion

        #region LanguageDetailsSeed
        modelBuilder.Entity<Language>().OwnsOne(p => p.Details).HasData(
            new { LanguageId = 1, Phonetic = false, Tonal = false, PhonemesCount = 44 },
            new { LanguageId = 2, Phonetic = false, Tonal = false, PhonemesCount = 36 },
            new { LanguageId = 3, Phonetic = true, Tonal = false, PhonemesCount = 24 });
        #endregion
    }
}
