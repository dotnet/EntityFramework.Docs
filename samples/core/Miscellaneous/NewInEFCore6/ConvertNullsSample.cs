using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public static class ConvertNullsSample
{
    public static async Task Value_converters_can_convert_nulls()
    {
        Console.WriteLine($">>>> Sample: {nameof(Value_converters_can_convert_nulls)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();

        using (var context = new CatsContext())
        {
            #region InsertCats
            context.AddRange(
                new Cat { Name = "Mac", Breed = Breed.Unknown },
                new Cat { Name = "Clippy", Breed = Breed.Burmese },
                new Cat { Name = "Sid", Breed = Breed.Tonkinese });

            await context.SaveChangesAsync();
            #endregion

            Console.WriteLine();
        }

        using (var context = new CatsContext())
        {
            var cats = await context.Cats.ToListAsync();

            Console.WriteLine();

            foreach (var cat in cats)
            {
                Console.WriteLine($"{cat.Name} has breed '{cat.Breed}'.");
            }
        }

        Console.WriteLine();
    }

    public static async Task Value_converters_can_convert_nulls_in_FKs()
    {
        Console.WriteLine($">>>> Sample: {nameof(Value_converters_can_convert_nulls_in_FKs)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();

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

            await context.SaveChangesAsync();

            Console.WriteLine();
        }

        using (var context = new CarsContext())
        {
            // Not currently working
            var cars = await context.Cars.Where(e => e.OwnerId == null).ToListAsync();

            Console.WriteLine();

            foreach (var car in cars)
            {
                Console.WriteLine($"The {car.Model} does not have an owner.");
            }
        }

        Console.WriteLine();
    }

    public static class Helpers
    {
        public static async Task RecreateCleanDatabase()
        {
            using (var context = new CarsContext(quiet: true))
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.MigrateAsync();
            }

            using (var context = new CatsContext(quiet: true))
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
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
        private readonly bool _quiet;

        public CarsContext(bool quiet = false)
        {
            _quiet = quiet;
        }

        public DbSet<Car> Cars { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCoreSample;ConnectRetryCount=0");

            if (!_quiet)
            {
                optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Car>()
                .Property(e => e.OwnerId)
                .HasConversion<ZeroToNullConverter>();
        }
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
        private readonly bool _quiet;

        public CatsContext(bool quiet = false)
        {
            _quiet = quiet;
        }

        public DbSet<Cat> Cats { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCoreSample;ConnectRetryCount=0");

            if (!_quiet)
            {
                optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Cat>()
                .Property(e => e.Breed)
                .HasConversion<BreedConverter>();
        }
    }
}

namespace CarsMigrations
{
    [DbContext(typeof(ConvertNullsSample.CarsContext))]
    [Migration("20210927174004_Cars")]
    public partial class Cars : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                    {
                        table.PrimaryKey("PK_Person", x => x.Id);
                    });

            migrationBuilder.CreateTable(
                name: "Cars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                    {
                        table.PrimaryKey("PK_Cars", x => x.Id);
                    });

            migrationBuilder.CreateIndex(
                name: "IX_Cars_OwnerId",
                table: "Cars",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cars");

            migrationBuilder.DropTable(
                name: "Person");
        }
    }

    [DbContext(typeof(ConvertNullsSample.CarsContext))]
    partial class CarsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0-rc.1.21452.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("ConvertNullsSample+Car", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

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

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

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

            modelBuilder.Entity("ConvertNullsSample+Person", b =>
                {
                    b.Navigation("Cars");
                });
#pragma warning restore 612, 618
        }
    }
}
