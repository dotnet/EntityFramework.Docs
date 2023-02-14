using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

public static class CosmosModelConfigurationSample
{
    public static void Cosmos_configure_time_to_live()
    {
        Console.WriteLine($">>>> Sample: {nameof(Cosmos_configure_time_to_live)}");
        Console.WriteLine();

        using var context = new FamilyContext();

        // Don't run on the emulator since some options are not supported there.
        Console.WriteLine(context.Model.ToDebugString());

        Console.WriteLine();
    }

    public static async Task Cosmos_configure_time_to_live_per_instance()
    {
        Console.WriteLine($">>>> Sample: {nameof(Cosmos_configure_time_to_live_per_instance)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await using var context = new PlacesContext();

        #region SetTtl
        var village = new Village { Id = "DN41", Name = "Healing", TimeToLive = 60 };
        context.Add(village);
        await context.SaveChangesAsync();
        #endregion

        #region SetTtlShadow
        var hamlet = new Hamlet { Id = "DN37", Name = "Irby" };
        context.Add(hamlet);
        context.Entry(hamlet).Property("TimeToLive").CurrentValue = 60;
        await context.SaveChangesAsync();
        #endregion

        Console.WriteLine();
    }

    public class Family
    {
        public string Id { get; set; }
        public string LastName { get; set; }
        public IList<Parent> Parents { get; set; }
        public IList<Child> Children { get; set; }
        public Address Address { get; set; }
        public bool IsRegistered { get; set; }
    }

    public class Village
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int TimeToLive { get; set; }
    }

    public class Hamlet
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class Parent
    {
        public string FamilyName { get; set; }
        public string FirstName { get; set; }
    }

    public class Child
    {
        public string FamilyName { get; set; }
        public string FirstName { get; set; }
        public string Gender { get; set; }
        public int Grade { get; set; }
        public IList<Pet> Pets { get; set; }
    }

    public class Pet
    {
        public string GivenName { get; set; }
    }

    public class Address
    {
        public string State { get; set; }
        public string County { get; set; }
        public string City { get; set; }
    }

    public class FamilyContext : DbContext
    {
        public DbSet<Family> Families { get; set; }

        private readonly bool _quiet;

        public FamilyContext(bool quiet = false)
        {
            _quiet = quiet;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Family>(
                entityTypeBuilder =>
                {
                    entityTypeBuilder.Property(e => e.Id).ToJsonProperty("id");
                    entityTypeBuilder.HasPartitionKey(e => e.LastName);
                });

            #region ModelThroughput
            modelBuilder.HasManualThroughput(2000);
            modelBuilder.HasAutoscaleThroughput(4000);
            #endregion

            #region EntityTypeThroughput
            modelBuilder.Entity<Family>(
                entityTypeBuilder =>
                {
                    entityTypeBuilder.HasManualThroughput(5000);
                    entityTypeBuilder.HasAutoscaleThroughput(3000);
                });
            #endregion

            #region TimeToLive
            modelBuilder.Entity<Family>(
                entityTypeBuilder =>
                {
                    entityTypeBuilder.HasDefaultTimeToLive(100);
                    entityTypeBuilder.HasAnalyticalStoreTimeToLive(200);
                });
            #endregion
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseCosmos(
                    "https://localhost:8081",
                    "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                    "TODO");

            if (!_quiet)
            {
                optionsBuilder.LogTo(Console.WriteLine, new[] { CosmosEventId.ExecutingSqlQuery });
            }
        }
    }

    public class PlacesContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region TimeToLiveProperty
            modelBuilder.Entity<Village>()
                .HasDefaultTimeToLive(3600)
                .Property(e => e.TimeToLive)
                .ToJsonProperty("ttl");
            #endregion

            #region TimeToLiveShadowProperty
            modelBuilder.Entity<Hamlet>()
                .HasDefaultTimeToLive(3600)
                .Property<int>("TimeToLive")
                .ToJsonProperty("ttl");
            #endregion
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .EnableSensitiveDataLogging()
                .LogTo(Console.WriteLine, LogLevel.Information)
                .UseCosmos(
                    "https://localhost:8081",
                    "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                    "Places");
    }

    public static class Helpers
    {
        public static async Task RecreateCleanDatabase()
        {
            await using var context = new PlacesContext();

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }
    }
}
