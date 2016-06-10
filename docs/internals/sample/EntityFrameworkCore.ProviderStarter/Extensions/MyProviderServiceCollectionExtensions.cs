using EntityFrameworkCore.ProviderStarter.Infrastructure;
using EntityFrameworkCore.ProviderStarter.Query;
using EntityFrameworkCore.ProviderStarter.Query.ExpressionVisitors;
using EntityFrameworkCore.ProviderStarter.Storage;
using EntityFrameworkCore.ProviderStarter.ValueGeneration;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MyProviderServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityFrameworkMyProvider(this IServiceCollection services)
        {
            services.AddEntityFramework();

            services.TryAddEnumerable(ServiceDescriptor
                .Singleton<IDatabaseProvider, DatabaseProvider<MyDatabaseProviderServices, MyProviderOptionsExtension>>());

            services.TryAdd(new ServiceCollection()
                // singleton services
                .AddSingleton<MyModelSource>()
                .AddSingleton<MyValueGeneratorCache>()
                // scoped services
                .AddScoped<MyDatabaseProviderServices>()
                .AddScoped<MyDatabaseCreator>()
                .AddScoped<MyDatabase>()
                .AddScoped<MyEntityQueryableExpressionVisitorFactory>()
                .AddScoped<MyEntityQueryModelVisitorFactory>()
                .AddScoped<MyQueryContextFactory>()
                .AddScoped<MyTransactionManager>());

            return services;
        }
    }
}
