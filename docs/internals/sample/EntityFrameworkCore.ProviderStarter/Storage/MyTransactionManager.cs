using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.ProviderStarter.Storage
{
    public class MyTransactionManager : IDbContextTransactionManager
    {
        public IDbContextTransaction BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public Task<IDbContextTransaction> BeginTransactionAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public void CommitTransaction()
        {
            throw new NotImplementedException();
        }

        public void RollbackTransaction()
        {
            throw new NotImplementedException();
        }
    }
}