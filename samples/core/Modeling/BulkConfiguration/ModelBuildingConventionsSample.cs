using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable CS0169

namespace EFModeling.BulkConfiguration;

public static class ModelBuildingConventionsSample
{
    public static Task No_foreign_key_index_convention()
    {
        PrintSampleName();
        return ConventionsTest<NoForeignKeyIndexBlogsContext>();
    }

    public static Task Discriminator_length_convention()
    {
        PrintSampleName();
        return ConventionsTest<DiscriminatorLengthBlogsContext>();
    }

    public static Task Max_string_length_convention()
    {
        PrintSampleName();
        return ConventionsTest<MaxStringLengthBlogsContext>();
    }

    public static async Task Map_members_explicitly_by_attribute_convention()
    {
        PrintSampleName();

        await using var context = new LaundryContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        Console.WriteLine(context.Model.ToDebugString());
        Console.WriteLine();
    }

    private static async Task ConventionsTest<TContext>()
        where TContext : BlogsContext, new()
    {
        await using var context = new TContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.Seed();
        context.ChangeTracker.Clear();

        Console.WriteLine(context.Model.ToDebugString());

        var blogs = await context.Blogs
            .Include(blog => blog.Posts).ThenInclude(post => post.Author)
            .Include(blog => blog.Posts).ThenInclude(post => post.Tags)
            .ToListAsync();

        blogs[0].Name += "Changed";
        blogs[1].Posts[2].Content += "Changed";
        blogs[2].Posts[0].Author!.Contact.Address.Country = "United Kingdon";
        blogs[3].Posts[1].Tags.Add(new Tag("TagNt", "New Tag"));
        blogs[2].Posts[1].Tags.Remove(blogs[2].Posts[1].Tags[0]);

        await context.SaveChangesAsync();

        Console.WriteLine();
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }
}

public abstract class ModelBuildingBlogsContextBase : BlogsContext
{
    protected ModelBuildingBlogsContextBase()
        : base()
    {
    }

    public override MappingStrategy MappingStrategy => MappingStrategy.Tph;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>().OwnsOne(
            author => author.Contact, ownedNavigationBuilder =>
            {
                ownedNavigationBuilder.OwnsOne(contactDetails => contactDetails.Address);
            });

        modelBuilder.Entity<Post>().OwnsOne(
            post => post.Metadata, ownedNavigationBuilder =>
            {
                ownedNavigationBuilder.OwnsMany(metadata => metadata.TopSearches);
                ownedNavigationBuilder.OwnsMany(metadata => metadata.TopGeographies);
                ownedNavigationBuilder.OwnsMany(
                    metadata => metadata.Updates,
                    ownedOwnedNavigationBuilder => ownedOwnedNavigationBuilder.OwnsMany(update => update.Commits));
            });

        base.OnModelCreating(modelBuilder);
    }
}

public class NoForeignKeyIndexBlogsContext : ModelBuildingBlogsContextBase
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>(entityTypeBuilder => entityTypeBuilder.HasIndex("BlogId"));

        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Conventions.Remove(typeof(ForeignKeyIndexConvention));

        base.ConfigureConventions(configurationBuilder);
    }
}

public class DiscriminatorLengthBlogsContext : ModelBuildingBlogsContextBase
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Post>()
            .HasDiscriminator<string>("PostTypeDiscriminator")
            .HasValue<Post>("Post")
            .HasValue<FeaturedPost>("Featured");
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Conventions.Add(_ => new DiscriminatorLengthConvention3());

        base.ConfigureConventions(configurationBuilder);
    }
}

#region DiscriminatorLengthConvention1
public class DiscriminatorLengthConvention1 : IEntityTypeBaseTypeChangedConvention
{
    public void ProcessEntityTypeBaseTypeChanged(
        IConventionEntityTypeBuilder entityTypeBuilder,
        IConventionEntityType? newBaseType,
        IConventionEntityType? oldBaseType,
        IConventionContext<IConventionEntityType> context)
    {
        var discriminatorProperty = entityTypeBuilder.Metadata.FindDiscriminatorProperty();
        if (discriminatorProperty != null
            && discriminatorProperty.ClrType == typeof(string))
        {
            discriminatorProperty.Builder.HasMaxLength(24);
        }
    }
}
#endregion

#region DiscriminatorLengthConvention2
public class DiscriminatorLengthConvention2 : IModelFinalizingConvention
{
    public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
    {
        foreach (var entityType in modelBuilder.Metadata.GetEntityTypes()
                     .Where(entityType => entityType.BaseType == null))
        {
            var discriminatorProperty = entityType.FindDiscriminatorProperty();
            if (discriminatorProperty != null
                && discriminatorProperty.ClrType == typeof(string))
            {
                discriminatorProperty.Builder.HasMaxLength(24);
            }
        }
    }
}
#endregion

#region DiscriminatorLengthConvention3
public class DiscriminatorLengthConvention3 : IModelFinalizingConvention
{
    public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
    {
        foreach (var entityType in modelBuilder.Metadata.GetEntityTypes()
                     .Where(entityType => entityType.BaseType == null))
        {
            var discriminatorProperty = entityType.FindDiscriminatorProperty();
            if (discriminatorProperty != null
                && discriminatorProperty.ClrType == typeof(string))
            {
                var maxDiscriminatorValueLength =
                    entityType.GetDerivedTypesInclusive().Select(e => ((string)e.GetDiscriminatorValue()!).Length).Max();

                discriminatorProperty.Builder.HasMaxLength(maxDiscriminatorValueLength);
            }
        }
    }
}
#endregion

public class MaxStringLengthBlogsContext : ModelBuildingBlogsContextBase
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>()
            .Property(post => post.Content)
            .HasMaxLength(4000);

        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Conventions.Add(_ => new MaxStringLengthConvention());
        configurationBuilder.Conventions.Add(_ => new DiscriminatorLengthConvention3());

        base.ConfigureConventions(configurationBuilder);
    }
}

#region MaxStringLengthConvention
public class MaxStringLengthConvention : IModelFinalizingConvention
{
    public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
    {
        foreach (var property in modelBuilder.Metadata.GetEntityTypes()
                     .SelectMany(
                         entityType => entityType.GetDeclaredProperties()
                             .Where(
                                 property => property.ClrType == typeof(string))))
        {
            property.Builder.HasMaxLength(512);
        }
    }
}
#endregion

#region MaxStringLengthNonUnicodeConvention
public class MaxStringLengthNonUnicodeConvention : IModelFinalizingConvention
{
    public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
    {
        foreach (var property in modelBuilder.Metadata.GetEntityTypes()
                     .SelectMany(
                         entityType => entityType.GetDeclaredProperties()
                             .Where(
                                 property => property.ClrType == typeof(string))))
        {
            var propertyBuilder = property.Builder;
            if (propertyBuilder.CanSetMaxLength(512)
                && propertyBuilder.CanSetIsUnicode(false))
            {
                propertyBuilder.HasMaxLength(512)!.IsUnicode(false);
            }
        }
    }
}
#endregion

public class LaundryContext : DbContext
{
    public DbSet<LaundryBasket> LaundryBaskets => Set<LaundryBasket>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(@$"Server=(localdb)\mssqllocaldb;Database={GetType().Name};ConnectRetryCount=0");

    #region ReplaceConvention
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Conventions.Replace<PropertyDiscoveryConvention>(
            serviceProvider => new AttributeBasedPropertyDiscoveryConvention(
                serviceProvider.GetRequiredService<ProviderConventionSetBuilderDependencies>()));
    }
    #endregion
}

#region PersistAttribute
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class PersistAttribute : Attribute
{
}
#endregion

#region LaundryBasket
public class LaundryBasket
{
    [Persist]
    [Key]
    private readonly int _id;

    [Persist]
    public int TenantId { get; init; }

    public bool IsClean { get; set; }

    public List<Garment> Garments { get; } = new();
}

public class Garment
{
    public Garment(string name, string color)
    {
        Name = name;
        Color = color;
    }

    [Persist]
    [Key]
    private readonly int _id;

    [Persist]
    public int TenantId { get; init; }

    [Persist]
    public string Name { get; }

    [Persist]
    public string Color { get; }

    public bool IsClean { get; set; }

    public LaundryBasket? Basket { get; set; }
}
#endregion

#region AttributeBasedPropertyDiscoveryConvention
public class AttributeBasedPropertyDiscoveryConvention : PropertyDiscoveryConvention
{
    public AttributeBasedPropertyDiscoveryConvention(ProviderConventionSetBuilderDependencies dependencies)
        : base(dependencies)
    {
    }

    public override void ProcessEntityTypeAdded(
        IConventionEntityTypeBuilder entityTypeBuilder,
        IConventionContext<IConventionEntityTypeBuilder> context)
        => Process(entityTypeBuilder);

    public override void ProcessEntityTypeBaseTypeChanged(
        IConventionEntityTypeBuilder entityTypeBuilder,
        IConventionEntityType? newBaseType,
        IConventionEntityType? oldBaseType,
        IConventionContext<IConventionEntityType> context)
    {
        if ((newBaseType == null
             || oldBaseType != null)
            && entityTypeBuilder.Metadata.BaseType == newBaseType)
        {
            Process(entityTypeBuilder);
        }
    }

    private void Process(IConventionEntityTypeBuilder entityTypeBuilder)
    {
        foreach (var memberInfo in GetRuntimeMembers())
        {
            if (Attribute.IsDefined(memberInfo, typeof(PersistAttribute), inherit: true))
            {
                entityTypeBuilder.Property(memberInfo);
            }
            else if (memberInfo is PropertyInfo propertyInfo
                     && Dependencies.TypeMappingSource.FindMapping(propertyInfo) != null)
            {
                entityTypeBuilder.Ignore(propertyInfo.Name);
            }
        }

        IEnumerable<MemberInfo> GetRuntimeMembers()
        {
            var clrType = entityTypeBuilder.Metadata.ClrType;

            foreach (var property in clrType.GetRuntimeProperties()
                         .Where(p => p.GetMethod != null && !p.GetMethod.IsStatic))
            {
                yield return property;
            }

            foreach (var property in clrType.GetRuntimeFields())
            {
                yield return property;
            }
        }
    }
}
#endregion
