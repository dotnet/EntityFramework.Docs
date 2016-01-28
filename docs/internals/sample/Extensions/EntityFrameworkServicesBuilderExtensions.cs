using EntityFrameworkCore.ProviderStarter.Infrastructure;
using EntityFrameworkCore.ProviderStarter.Storage;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EntityFrameworkServicesBuilderExtensions
    {
        public static EntityFrameworkServicesBuilder AddMyProvider(this EntityFrameworkServicesBuilder builder)
        {
            var serviceCollection = builder.GetInfrastructure();

            // Registering IDatabaseProvider is required
            // TryAddEnumerable is used here because a user may have multiple providers in the collection.
            // When DbContext creates its set of scoped services, it will only include the services
            // for the provider it is using.
            serviceCollection.TryAddEnumerable(ServiceDescriptor
                .Singleton<IDatabaseProvider, DatabaseProvider<MyDatabaseProviderServices, MyProviderOptionsExtension>>());

            // This is where providers register their custom services.
            serviceCollection.TryAdd(new ServiceCollection()
                    .AddScoped<MyDatabaseProviderServices>());

            return builder;
        }
    }
}
