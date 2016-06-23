using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.RelationalProviderStarter.Migrations
{
    public class MyHistoryRepository : HistoryRepository
    {
        public MyHistoryRepository(
            IDatabaseCreator databaseCreator,
            IRawSqlCommandBuilder rawSqlCommandBuilder,
            IRelationalConnection connection,
            IDbContextOptions options,
            IMigrationsModelDiffer modelDiffer,
            IMigrationsSqlGenerator migrationsSqlGenerator,
            IRelationalAnnotationProvider annotations,
            ISqlGenerationHelper sqlGenerationHelper)
            : base(databaseCreator,
                rawSqlCommandBuilder,
                connection,
                options,
                modelDiffer,
                migrationsSqlGenerator,
                annotations,
                sqlGenerationHelper
                )
        {
        }

        protected override string ExistsSql
        {
            get { throw new NotImplementedException(); }
        }

        public override string GetBeginIfExistsScript(string migrationId)
        {
            throw new NotImplementedException();
        }

        public override string GetBeginIfNotExistsScript(string migrationId)
        {
            throw new NotImplementedException();
        }

        public override string GetCreateIfNotExistsScript()
        {
            throw new NotImplementedException();
        }

        public override string GetEndIfScript()
        {
            throw new NotImplementedException();
        }

        protected override bool InterpretExistsResult(object value)
        {
            throw new NotImplementedException();
        }
    }
}