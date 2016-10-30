using System;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Internal;

namespace EntityFrameworkCore.ProviderStarter.Query
{
    public class MyQueryContextFactory : QueryContextFactory
    {
        public MyQueryContextFactory(IStateManager stateManager, 
            IConcurrencyDetector concurrencyDetector, 
            IChangeDetector changeDetector) 
            : base(stateManager, concurrencyDetector, changeDetector)
        {
        }

        public override QueryContext Create()
        {
            throw new NotImplementedException();
        }
    }
}