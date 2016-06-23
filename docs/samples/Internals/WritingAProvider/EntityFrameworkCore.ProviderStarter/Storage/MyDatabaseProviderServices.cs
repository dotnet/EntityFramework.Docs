using System;
using EntityFrameworkCore.ProviderStarter.Infrastructure;
using EntityFrameworkCore.ProviderStarter.Query;
using EntityFrameworkCore.ProviderStarter.Query.ExpressionVisitors;
using EntityFrameworkCore.ProviderStarter.ValueGeneration;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace EntityFrameworkCore.ProviderStarter.Storage
{
    public class MyDatabaseProviderServices : DatabaseProviderServices
    {
        public MyDatabaseProviderServices(IServiceProvider services)
            : base(services)
        {
        }

        public override string InvariantName
            => "MyProvider";

        public override IDatabaseCreator Creator
            => GetService<MyDatabaseCreator>();

        public override IDatabase Database
            => GetService<MyDatabase>();

        public override IEntityQueryableExpressionVisitorFactory EntityQueryableExpressionVisitorFactory
            => GetService<MyEntityQueryableExpressionVisitorFactory>();

        public override IEntityQueryModelVisitorFactory EntityQueryModelVisitorFactory
            => GetService<MyEntityQueryModelVisitorFactory>();

        public override IModelSource ModelSource
            => GetService<MyModelSource>();

        public override IQueryContextFactory QueryContextFactory
            => GetService<MyQueryContextFactory>();

        public override IDbContextTransactionManager TransactionManager
            => GetService<MyTransactionManager>();

        public override IValueGeneratorCache ValueGeneratorCache
            => GetService<MyValueGeneratorCache>();
    }
}