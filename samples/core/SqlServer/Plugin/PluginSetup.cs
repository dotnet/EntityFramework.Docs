using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;

namespace SqlServer.Plugin;
public class AugmentationDbFunctionsExtensions : IDbContextOptionsExtension
{
    private ExtensionInfo _info;

    public virtual DbContextOptionsExtensionInfo Info
        => _info ??= new ExtensionInfo(this);

    public void ApplyServices(IServiceCollection services) => services.AddAugmentationExtension();

    public void Validate(IDbContextOptions options)
    {
    }

    private sealed class ExtensionInfo : DbContextOptionsExtensionInfo
    {
        public ExtensionInfo(IDbContextOptionsExtension extension)
            : base(extension)
        {
        }

        private new AugmentationDbFunctionsExtensions Extension
            => (AugmentationDbFunctionsExtensions)base.Extension;

        public override bool IsDatabaseProvider
            => false;

        public override int GetServiceProviderHashCode()
            => 0;

        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
            => other is ExtensionInfo;

        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            => debugInfo["SqlServer:" + nameof(SqlServerExtensions.UseAugmentation)] = "1";

        public override string LogFragment
            => "using Augmentation";
    }
}

public static class AugmentationServiceCollectionExtensions
{
    public static IServiceCollection AddAugmentationExtension(
        this IServiceCollection serviceCollection)
    {
        new EntityFrameworkRelationalServicesBuilder(serviceCollection)
            .TryAdd<IMethodCallTranslatorPlugin, AugmentationTranslatorPlugin>();

        return serviceCollection;
    }
}

public class AugmentationTranslatorPlugin : IMethodCallTranslatorPlugin
{
    public IEnumerable<IMethodCallTranslator> Translators { get; }

    public AugmentationTranslatorPlugin()
    {
        var list = new List<IMethodCallTranslator>
        {
            new AugmentationTranslator()
        };
        Translators = list;
    }
}

public static class SqlServerExtensions
{
    public static SqlServerDbContextOptionsBuilder UseAugmentation(this SqlServerDbContextOptionsBuilder optionsBuilder)
    {
        var coreOptionsBuilder = ((IRelationalDbContextOptionsBuilderInfrastructure)optionsBuilder).OptionsBuilder;
        var extension = coreOptionsBuilder.Options.FindExtension<AugmentationDbFunctionsExtensions>() ?? new AugmentationDbFunctionsExtensions();
        ((IDbContextOptionsBuilderInfrastructure)coreOptionsBuilder).AddOrUpdateExtension(extension);

        return optionsBuilder;
    }
}
