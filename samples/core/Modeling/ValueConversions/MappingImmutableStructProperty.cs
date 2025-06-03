using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EFModeling.ValueConversions;

public class MappingImmutableStructProperty : Program
{
    public async Task Run()
    {
        ConsoleWriteLines("Sample showing value conversions for a simple immutable struct...");

        using (var context = new SampleDbContext())
        {
            await CleanDatabase(context);

            ConsoleWriteLines("Save a new entity...");

            var entity = new EntityType { MyProperty = new ImmutableStruct(6) };
            context.Add(entity);
            await context.SaveChangesAsync();

            ConsoleWriteLines("Change the property value and save again...");

            // This will be detected and EF will update the database on SaveChanges
            entity.MyProperty = new ImmutableStruct(66);

            await context.SaveChangesAsync();
        }

        using (var context = new SampleDbContext())
        {
            ConsoleWriteLines("Read the entity back...");

            var entity = await context.Set<EntityType>().SingleAsync();

            Debug.Assert(entity.MyProperty.Value == 66);
        }

        ConsoleWriteLines("Sample finished.");
    }

    public class SampleDbContext : DbContext
    {
        private static readonly ILoggerFactory
            Logger = LoggerFactory.Create(x => x.AddConsole()); //.SetMinimumLevel(LogLevel.Debug));

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region ConfigureImmutableStructProperty
            modelBuilder
                .Entity<EntityType>()
                .Property(e => e.MyProperty)
                .HasConversion(
                    v => v.Value,
                    v => new ImmutableStruct(v));
            #endregion
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseLoggerFactory(Logger)
                .UseSqlite("DataSource=test.db")
                .EnableSensitiveDataLogging();
    }

    public class EntityType
    {
        public int Id { get; set; }
        public ImmutableStruct MyProperty { get; set; }
    }

    #region SimpleImmutableStruct
    public readonly struct ImmutableStruct
    {
        public ImmutableStruct(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
    #endregion
}
