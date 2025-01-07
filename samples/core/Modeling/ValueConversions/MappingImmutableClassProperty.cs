using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EFModeling.ValueConversions;

public class MappingImmutableClassProperty : Program
{
    public async Task Run()
    {
        ConsoleWriteLines("Sample showing value conversions for a simple immutable class...");

        using (var context = new SampleDbContext())
        {
            await CleanDatabase(context);

            ConsoleWriteLines("Save a new entity...");

            var entity = new MyEntityType { MyProperty = new ImmutableClass(7) };
            context.Add(entity);
            await context.SaveChangesAsync();

            ConsoleWriteLines("Change the property value and save again...");

            // This will be detected and EF will update the database on SaveChanges
            entity.MyProperty = new ImmutableClass(77);

            await context.SaveChangesAsync();
        }

        using (var context = new SampleDbContext())
        {
            ConsoleWriteLines("Read the entity back...");

            var entity = await context.Set<MyEntityType>().SingleAsync();

            Debug.Assert(entity.MyProperty.Value == 77);
        }

        ConsoleWriteLines("Sample finished.");
    }

    public class SampleDbContext : DbContext
    {
        private static readonly ILoggerFactory
            Logger = LoggerFactory.Create(x => x.AddConsole()); //.SetMinimumLevel(LogLevel.Debug));

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region ConfigureImmutableClassProperty
            modelBuilder
                .Entity<MyEntityType>()
                .Property(e => e.MyProperty)
                .HasConversion(
                    v => v.Value,
                    v => new ImmutableClass(v));
            #endregion
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseLoggerFactory(Logger)
                .UseSqlite("DataSource=test.db")
                .EnableSensitiveDataLogging();
    }

    public class MyEntityType
    {
        public int Id { get; set; }
        public ImmutableClass MyProperty { get; set; }
    }

    #region SimpleImmutableClass
    public sealed class ImmutableClass
    {
        public ImmutableClass(int value)
        {
            Value = value;
        }

        public int Value { get; }

        private bool Equals(ImmutableClass other)
            => Value == other.Value;

        public override bool Equals(object obj)
            => ReferenceEquals(this, obj) || obj is ImmutableClass other && Equals(other);

        public override int GetHashCode()
            => Value.GetHashCode();
    }
    #endregion
}
