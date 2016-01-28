using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace EntityFrameworkCore.ProviderStarter.ValueGeneration
{
    internal class MyValueGeneratorCache : IValueGeneratorCache
    {
        public ValueGenerator GetOrAdd(IProperty property, IEntityType entityType, Func<IProperty, IEntityType, ValueGenerator> factory)
        {
            throw new NotImplementedException();
        }
    }
}