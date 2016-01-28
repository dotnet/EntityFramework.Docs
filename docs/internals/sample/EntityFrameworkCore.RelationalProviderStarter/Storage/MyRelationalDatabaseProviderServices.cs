using System;
using EntityFrameworkCore.RelationalProviderStarter.Infrastructure;
using EntityFrameworkCore.RelationalProviderStarter.Metadata;
using EntityFrameworkCore.RelationalProviderStarter.Migrations;
using EntityFrameworkCore.RelationalProviderStarter.Query.ExpressionTranslators.Internal;
using EntityFrameworkCore.RelationalProviderStarter.Query.Sql;
using EntityFrameworkCore.RelationalProviderStarter.Update;
using EntityFrameworkCore.RelationalProviderStarter.ValueGeneration;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace EntityFrameworkCore.RelationalProviderStarter.Storage
{
    public class MyRelationalDatabaseProviderServices : RelationalDatabaseProviderServices
    {
        public MyRelationalDatabaseProviderServices(IServiceProvider services)
            : base(services)
        {
        }

        public override string InvariantName
            => "MyRelationalProvider";

        public override IRelationalAnnotationProvider AnnotationProvider
            => GetService<MyRelationalAnnotationProvider>();

        public override IMemberTranslator CompositeMemberTranslator
            => GetService<MyRelationalCompositeMemberTranslator>();

        public override IMethodCallTranslator CompositeMethodCallTranslator
            => GetService<MyRelationalCompositeMethodCallTranslator>();


        public override IDatabaseCreator Creator
            => GetService<MyRelationalDatabaseCreator>();

        public override IHistoryRepository HistoryRepository
            => GetService<MyHistoryRepository>();

        public override IModelSource ModelSource
            => GetService<MyModelSource>();

        public override IModificationCommandBatchFactory ModificationCommandBatchFactory
            => GetService<MyModificationCommandBatchFactory>();

        public override IQuerySqlGeneratorFactory QuerySqlGeneratorFactory
            => GetService<MyQuerySqlGeneratorFactory>();

        public override IRelationalConnection RelationalConnection
            => GetService<MyRelationalConnection>();

        public override IRelationalDatabaseCreator RelationalDatabaseCreator
            => GetService<MyRelationalDatabaseCreator>();

        public override ISqlGenerationHelper SqlGenerationHelper
            => GetService<MyRelationalSqlGenerationHelper>();

        public override IUpdateSqlGenerator UpdateSqlGenerator
            => GetService<MyUpdateSqlGenerator>();

        public override IValueGeneratorCache ValueGeneratorCache
            => GetService<MyValueGeneratorCache>();
    }
}