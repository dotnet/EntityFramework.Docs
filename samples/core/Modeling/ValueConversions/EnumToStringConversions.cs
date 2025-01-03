using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EFModeling.ValueConversions;

public class EnumToStringConversions : Program
{
    public async Task Run()
    {
        ConsoleWriteLines("Sample showing explicitly configured value converter");

        using (var context = new SampleDbContextExplicit())
        {
            await CleanDatabase(context);

            context.Add(new Rider { Mount = EquineBeast.Horse });
            await context.SaveChangesAsync();
        }

        using (var context = new SampleDbContextExplicit())
        {
            ConsoleWriteLines($"Enum value read as '{(await context.Set<Rider>().SingleAsync()).Mount}'.");
        }

        ConsoleWriteLines("Sample showing conversion configured by CLR type");

        using (var context = new SampleDbContextByClrType())
        {
            await CleanDatabase(context);

            context.Add(new Rider { Mount = EquineBeast.Horse });
            await context.SaveChangesAsync();
        }

        using (var context = new SampleDbContextByClrType())
        {
            ConsoleWriteLines($"Enum value read as '{(await context.Set<Rider>().SingleAsync()).Mount}'.");
        }

        ConsoleWriteLines("Sample showing conversion configured by database type");

        using (var context = new SampleDbContextByDatabaseType())
        {
            await CleanDatabase(context);

            context.Add(new Rider2 { Mount = EquineBeast.Horse });
            await context.SaveChangesAsync();
        }

        using (var context = new SampleDbContextByDatabaseType())
        {
            ConsoleWriteLines($"Enum value read as '{(await context.Set<Rider2>().SingleAsync()).Mount}'.");
        }

        ConsoleWriteLines("Sample showing conversion configured by a ValueConverter instance");

        using (var context = new SampleDbContextByConverterInstance())
        {
            await CleanDatabase(context);

            context.Add(new Rider { Mount = EquineBeast.Horse });
            await context.SaveChangesAsync();
        }

        using (var context = new SampleDbContextByConverterInstance())
        {
            ConsoleWriteLines($"Enum value read as '{(await context.Set<Rider>().SingleAsync()).Mount}'.");
        }

        ConsoleWriteLines("Sample showing conversion configured by a built-in ValueConverter instance");

        using (var context = new SampleDbContextByBuiltInInstance())
        {
            await CleanDatabase(context);

            context.Add(new Rider { Mount = EquineBeast.Horse });
            await context.SaveChangesAsync();
        }

        using (var context = new SampleDbContextByBuiltInInstance())
        {
            ConsoleWriteLines($"Enum value read as '{(await context.Set<Rider>().SingleAsync()).Mount}'.");
        }

        ConsoleWriteLines("Sample showing conversion configured by CLR type with per-property facets");

        using (var context = new SampleDbContextByClrTypeWithFacets())
        {
            await CleanDatabase(context);

            context.Add(new Rider { Mount = EquineBeast.Horse });
            await context.SaveChangesAsync();
        }

        using (var context = new SampleDbContextByClrTypeWithFacets())
        {
            ConsoleWriteLines($"Enum value read as '{(await context.Set<Rider>().SingleAsync()).Mount}'.");
        }

        ConsoleWriteLines("Sample showing conversion configured by a ValueConverter instance with per-property facets");

        using (var context = new SampleDbContextByConverterInstanceWithFacets())
        {
            await CleanDatabase(context);

            context.Add(new Rider { Mount = EquineBeast.Horse });
            await context.SaveChangesAsync();
        }

        using (var context = new SampleDbContextByConverterInstanceWithFacets())
        {
            ConsoleWriteLines($"Enum value read as '{(await context.Set<Rider>().SingleAsync()).Mount}'.");
        }
    }

    public class SampleDbContextExplicit : SampleDbContextBase
    {
        #region ExplicitConversion
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Rider>()
                .Property(e => e.Mount)
                .HasConversion(
                    v => v.ToString(),
                    v => (EquineBeast)Enum.Parse(typeof(EquineBeast), v));
        }
        #endregion
    }

    public class SampleDbContextByClrType : SampleDbContextBase
    {
        #region ConversionByClrType
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Rider>()
                .Property(e => e.Mount)
                .HasConversion<string>();
        }
        #endregion
    }

    public class SampleDbContextByDatabaseType : SampleDbContextBase
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rider2>();
        }
    }

    public class SampleDbContextByConverterInstance : SampleDbContextBase
    {
        #region ConversionByConverterInstance
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var converter = new ValueConverter<EquineBeast, string>(
                v => v.ToString(),
                v => (EquineBeast)Enum.Parse(typeof(EquineBeast), v));

            modelBuilder
                .Entity<Rider>()
                .Property(e => e.Mount)
                .HasConversion(converter);
        }
        #endregion
    }

    public class SampleDbContextByClrTypeWithFacets : SampleDbContextBase
    {
        #region ConversionByClrTypeWithFacets
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Rider>()
                .Property(e => e.Mount)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsUnicode(false);
        }
        #endregion
    }

    public class SampleDbContextByConverterInstanceWithFacets : SampleDbContextBase
    {
        #region ConversionByConverterInstanceWithFacets
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var converter = new ValueConverter<EquineBeast, string>(
                v => v.ToString(),
                v => (EquineBeast)Enum.Parse(typeof(EquineBeast), v));

            modelBuilder
                .Entity<Rider>()
                .Property(e => e.Mount)
                .HasConversion(converter)
                .HasMaxLength(20)
                .IsUnicode(false);
        }
        #endregion
    }

    public class SampleDbContextByConverterInstanceWithMappingHints : SampleDbContextBase
    {
        #region ConversionByConverterInstanceWithMappingHints
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var converter = new ValueConverter<EquineBeast, string>(
                v => v.ToString(),
                v => (EquineBeast)Enum.Parse(typeof(EquineBeast), v),
                new ConverterMappingHints(size: 20, unicode: false));

            modelBuilder
                .Entity<Rider>()
                .Property(e => e.Mount)
                .HasConversion(converter);
        }
        #endregion
    }

    public class SampleDbContextByBuiltInInstance : SampleDbContextBase
    {
        #region ConversionByBuiltInInstance
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var converter = new EnumToStringConverter<EquineBeast>();

            modelBuilder
                .Entity<Rider>()
                .Property(e => e.Mount)
                .HasConversion(converter);
        }
        #endregion
    }

    public class SampleDbContextBoolToInt : SampleDbContextBase
    {
        #region ConversionByBuiltInBoolToInt
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<User>()
                .Property(e => e.IsActive)
                .HasConversion<int>();
        }
        #endregion
    }

    public class SampleDbContextBoolToIntExplicit : SampleDbContextBase
    {
        #region ConversionByBuiltInBoolToIntExplicit
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var converter = new BoolToZeroOneConverter<int>();

            modelBuilder
                .Entity<User>()
                .Property(e => e.IsActive)
                .HasConversion(converter);
        }
        #endregion
    }

    public class SampleDbContextRider2 : SampleDbContextBase
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region ConversionByDatabaseTypeFluent
            modelBuilder
                .Entity<Rider2>()
                .Property(e => e.Mount)
                .HasColumnType("nvarchar(24)");
            #endregion
        }
    }
    public class SampleDbContextBase : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted })
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EnumConversions;Trusted_Connection=True;ConnectRetryCount=0")
                .EnableSensitiveDataLogging();
    }

    #region BeastAndRider
    public class Rider
    {
        public int Id { get; set; }
        public EquineBeast Mount { get; set; }
    }

    public enum EquineBeast
    {
        Donkey,
        Mule,
        Horse,
        Unicorn
    }
    #endregion

    #region ConversionByDatabaseType
    public class Rider2
    {
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(24)")]
        public EquineBeast Mount { get; set; }
    }
    #endregion

    public class User
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }
}
