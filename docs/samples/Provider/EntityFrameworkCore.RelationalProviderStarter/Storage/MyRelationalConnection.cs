using System;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkCore.RelationalProviderStarter.Storage
{
    public class MyRelationalConnection : RelationalConnection
    {
        public MyRelationalConnection(IDbContextOptions options, ILogger logger)
            : base(options, logger)
        {
        }

        protected override DbConnection CreateDbConnection()
        {
            throw new NotImplementedException();
        }
    }
}