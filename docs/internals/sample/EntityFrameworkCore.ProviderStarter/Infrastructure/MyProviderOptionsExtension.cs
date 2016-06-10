using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.ProviderStarter.Infrastructure
{
    public class MyProviderOptionsExtension : IDbContextOptionsExtension
    {
        public string ConnectionString { get; set; }

        public void ApplyServices(IServiceCollection builder)
        {
            builder.AddEntityFrameworkMyProvider();
        }
    }
}