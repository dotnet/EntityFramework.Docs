using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.ProviderStarter.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.ProviderStarter.Storage
{
    public class MyDatabaseCreator : IDatabaseCreator
    {
        private MyProviderOptionsExtension _myOptions;

        public MyDatabaseCreator(IDbContextOptions options)
        {
            _myOptions = options.FindExtension<MyProviderOptionsExtension>();
        }

        public bool EnsureCreated()
        {
            throw new NotImplementedException();
        }

        public Task<bool> EnsureCreatedAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public bool EnsureDeleted()
        {
            throw new NotImplementedException();
        }

        public Task<bool> EnsureDeletedAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}