using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace NewInEfCore9;

public static class CustomConventionsSample
{
    public static async Task Conventions_enhancements_in_EF9()
    {
        PrintSampleName();

        await using var context = new TestContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.Seed();

        context.LoggingEnabled = true;
        context.ChangeTracker.Clear();

        Console.WriteLine(context.Model.ToDebugString());
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }

    public class TestContext : DbContext
    {
        public bool LoggingEnabled { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(@$"Server=(localdb)\mssqllocaldb;Database={GetType().Name};ConnectRetryCount=0")
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
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Replace<PropertyDiscoveryConvention>(
                serviceProvider => new AttributeBasedPropertyDiscoveryConvention(
                    serviceProvider.GetRequiredService<ProviderConventionSetBuilderDependencies>()));
        }

        public async Task Seed()
        {
            await SaveChangesAsync();
        }
    }

    #region AttributeBasedPropertyDiscoveryConvention
    public class AttributeBasedPropertyDiscoveryConvention(ProviderConventionSetBuilderDependencies dependencies)
        : PropertyDiscoveryConvention(dependencies)
    {
        protected override bool IsCandidatePrimitiveProperty(
            MemberInfo memberInfo, IConventionTypeBase structuralType, out CoreTypeMapping? mapping)
        {
            if (base.IsCandidatePrimitiveProperty(memberInfo, structuralType, out mapping))
            {
                if (Attribute.IsDefined(memberInfo, typeof(PersistAttribute), inherit: true))
                {
                    return true;
                }

                structuralType.Builder.Ignore(memberInfo.Name);
            }

            mapping = null;
            return false;
        }
    }
    #endregion

    #region Country
    public class Country
    {
        [Persist]
        public int Code { get; set; }

        [Persist]
        public required string Name { get; set; }

        public bool IsDirty { get; set; } // Will not be mapped by default.

        private class FooConfiguration : IEntityTypeConfiguration<Country>
        {
            private FooConfiguration()
            {
            }

            public void Configure(EntityTypeBuilder<Country> builder)
            {
                builder.HasKey(e => e.Code);
            }
        }
    }
    #endregion
}

#region PersistAttribute
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class PersistAttribute : Attribute
{
}
#endregion
