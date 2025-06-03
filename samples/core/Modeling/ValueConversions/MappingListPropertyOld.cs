using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace EFModeling.ValueConversions;

public class MappingListPropertyOld : Program
{
    public async Task Run()
    {
        ConsoleWriteLines("Sample showing value conversions for a List<int>...");

        using (var context = new SampleDbContext())
        {
            await CleanDatabase(context);

            ConsoleWriteLines("Save a new entity...");

            var entity = new EntityType { MyListProperty = new List<int> { 1, 2, 3 } };
            context.Add(entity);
            await context.SaveChangesAsync();

            ConsoleWriteLines("Mutate the property value and save again...");

            // This will be detected and EF will update the database on SaveChanges
            entity.MyListProperty.Add(4);

            await context.SaveChangesAsync();
        }

        using (var context = new SampleDbContext())
        {
            ConsoleWriteLines("Read the entity back...");

            var entity = await context.Set<EntityType>().SingleAsync();

            Debug.Assert(entity.MyListProperty.SequenceEqual(new List<int> { 1, 2, 3, 4 }));
        }

        ConsoleWriteLines("Sample finished.");
    }

    public class SampleDbContext : DbContext
    {
        private static readonly ILoggerFactory
            Logger = LoggerFactory.Create(x => x.AddConsole()); //.SetMinimumLevel(LogLevel.Debug));

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region ConfigureListProperty
            modelBuilder
                .Entity<EntityType>()
                .Property(e => e.MyListProperty)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions)null));

            var valueComparer = new ValueComparer<List<int>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList());

            modelBuilder
                .Entity<EntityType>()
                .Property(e => e.MyListProperty)
                .Metadata
                .SetValueComparer(valueComparer);
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

        #region ListProperty
        public List<int> MyListProperty { get; set; }
        #endregion
    }
}
