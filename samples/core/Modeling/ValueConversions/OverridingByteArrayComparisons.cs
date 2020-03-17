using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace EFModeling.ValueConversions
{
    public class OverridingByteArrayComparisons : Program
    {
        public void Run()
        {
            ConsoleWriteLines("Sample showing overriding byte array comparisons...");

            using (var context = new SampleDbContext())
            {
                CleanDatabase(context);

                ConsoleWriteLines("Save a new entity...");

                var entity = new EntityType { MyBytes = new byte[] { 1, 2, 3 } };
                context.Add(entity);
                context.SaveChanges();

                ConsoleWriteLines("Mutate the property value and save again...");

                // Normally mutating the byte array would not be detected by EF Core.
                // In this case it will be detected because the comparer in the model is overridden.
                entity.MyBytes[1] = 4;

                context.SaveChanges();
            }

            using (var context = new SampleDbContext())
            {
                ConsoleWriteLines("Read the entity back...");

                var entity = context.Set<EntityType>().Single();

                Debug.Assert(entity.MyBytes.SequenceEqual(new byte[] { 1, 4, 3 }));
            }

            ConsoleWriteLines("Sample finished.");
        }
        
        public class SampleDbContext : DbContext
        {
            private static readonly ILoggerFactory
                Logger = LoggerFactory.Create(x => x.AddConsole()); //.SetMinimumLevel(LogLevel.Debug));

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                #region OverrideComparer
                modelBuilder
                    .Entity<EntityType>()
                    .Property(e => e.MyBytes)
                    .Metadata
                    .SetValueComparer(new ValueComparer<byte[]>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToArray()));
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
        
            public byte[] MyBytes { get; set; }
        }
    }
}
