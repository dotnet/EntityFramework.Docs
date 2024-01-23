using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public static class ConvertNullsSample
{
    public static void Value_converters_can_convert_nulls()
    {
        Console.WriteLine($">>>> Sample: {nameof(Value_converters_can_convert_nulls)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();

        using (var context = new CatsContext())
        {
            #region InsertCats
            context.AddRange(
                new Cat { Name = "Mac", Breed = Breed.Unknown },
                new Cat { Name = "Clippy", Breed = Breed.Burmese },
                new Cat { Name = "Sid", Breed = Breed.Tonkinese });

            context.SaveChanges();
            #endregion

            Console.WriteLine();
        }

        using (var context = new CatsContext())
        {
            var cats = context.Cats.ToList();

            Console.WriteLine();

            foreach (Cat cat in cats)
            {
                Console.WriteLine($"{cat.Name} has breed '{cat.Breed}'.");
            }
        }

        Console.WriteLine();
    }

    public static void Value_converters_can_convert_nulls_in_FKs()
    {
        Console.WriteLine($">>>> Sample: {nameof(Value_converters_can_convert_nulls_in_FKs)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();

        using (var context = new CarsContext())
        {
            context.AddRange(
                new Car
                {
                    Model = "Toyota Yaris",
                    Owner = new() { Name = "Wendy" }
                },
                new Car
                {
                    Model = "Kia Soul"
                });

            context.SaveChanges();

            Console.WriteLine();
        }

        using (var context = new CarsContext())
        {
            // Not currently working
            var cars = context.Cars.Where(e => e.OwnerId == null).ToList();

            Console.WriteLine();

            foreach (Car car in cars)
            {
                Console.WriteLine($"The {car.Model} does not have an owner.");
            }
        }

        Console.WriteLine();
    }

    public static class Helpers
    {
        public static void RecreateCleanDatabase()
        {
            using (var context = new CarsContext(quiet: true))
            {
                context.Database.EnsureDeleted();
                context.Database.Migrate();
            }

            using (var context = new CatsContext(quiet: true))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
        }
    }

    #region PersonAndCar
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Car> Cars { get; set; }
    }

    public class Car
    {
        public int Id { get; set; }
        public string Model { get; set; }

        public int? OwnerId { get; set; }
        public Person Owner { get; set; }
    }
    #endregion

    public class ZeroToNullConverter : ValueConverter<int?, int>
    {
#pragma warning disable EF1001
        public ZeroToNullConverter()
            : base(
                v => v ?? 0,
                v => v == 0 ? null : v,
                convertsNulls: true)
        {
        }
#pragma warning restore EF1001
    }

    public class CarsContext : DbContext
    {
        readonly bool _quiet;

        public CarsContext(bool quiet = false) => _quiet = quiet;

        public DbSet<Car> Cars { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCoreSample");

            if (!_quiet)
            {
                optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) =>
            modelBuilder
                .Entity<Car>()
                .Property(e => e.OwnerId)
                .HasConversion<ZeroToNullConverter>();
    }

    public class Cat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Breed? Breed { get; set; }
    }

    #region Breed
    public enum Breed
    {
        Unknown,
        Burmese,
        Tonkinese
    }
    #endregion

    #region BreedConverter
    public class BreedConverter : ValueConverter<Breed, string>
    {
#pragma warning disable EF1001
        public BreedConverter()
            : base(
                v => v == Breed.Unknown ? null : v.ToString(),
                v => v == null ? Breed.Unknown : Enum.Parse<Breed>(v),
                convertsNulls: true)
        {
        }
#pragma warning restore EF1001
    }
    #endregion

    public class CatsContext : DbContext
    {
        readonly bool _quiet;

        public CatsContext(bool quiet = false) => _quiet = quiet;

        public DbSet<Cat> Cats { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCoreSample");

            if (!_quiet)
            {
                optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) =>
            modelBuilder
                .Entity<Cat>()
                .Property(e => e.Breed)
                .HasConversion<BreedConverter>();
    }
}

class CarsContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasAnnotation("ProductVersion", "6.0.0-rc.1.21452.10")
            .HasAnnotation("Relational:MaxIdentifierLength", 128);

        modelBuilder.UseIdentityColumns(1L, 1);

        modelBuilder.Entity("ConvertNullsSample+Car", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                b.Property<int>("Id").UseIdentityColumn(1L, 1);

                b.Property<string>("Model")
                    .HasColumnType("nvarchar(max)");

                b.Property<int?>("OwnerId")
                    .HasColumnType("int");

                b.HasKey("Id");

                b.HasIndex("OwnerId");

                b.ToTable("Cars");
            });

        modelBuilder.Entity("ConvertNullsSample+Person", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                b.Property<int>("Id").UseIdentityColumn(1L, 1);

                b.Property<string>("Name")
                    .HasColumnType("nvarchar(max)");

                b.HasKey("Id");

                b.ToTable("Person");
            });

        modelBuilder.Entity("ConvertNullsSample+Car", b =>
            {
                b.HasOne("ConvertNullsSample+Person", "Owner")
                    .WithMany("Cars")
                    .HasForeignKey("OwnerId");

                b.Navigation("Owner");
            });

        modelBuilder.Entity("ConvertNullsSample+Person", b => b.Navigation("Cars"));
    }
}
