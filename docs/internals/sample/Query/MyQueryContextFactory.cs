using System;
using Microsoft.EntityFrameworkCore.Query;

namespace EntityFrameworkCore.ProviderStarter.Query
{
    public class MyQueryContextFactory : IQueryContextFactory
    {
        public QueryContext Create()
        {
            throw new NotImplementedException();
        }
    }
}
