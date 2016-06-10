using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.RelationalProviderStarter.Storage
{
    public class MyRelationalDatabaseCreator : RelationalDatabaseCreator
    {
        public MyRelationalDatabaseCreator(
            IModel model,
            IRelationalConnection connection,
            IMigrationsModelDiffer modelDiffer,
            IMigrationsSqlGenerator migrationsSqlGenerator,
            IMigrationCommandExecutor migrationCommandExecutor)
            : base(model, connection, modelDiffer, migrationsSqlGenerator, migrationCommandExecutor)
        {
        }

        public override void Create()
        {
            throw new NotImplementedException();
        }

        public override void Delete()
        {
            throw new NotImplementedException();
        }

        public override bool Exists()
        {
            throw new NotImplementedException();
        }

        protected override bool HasTables()
        {
            throw new NotImplementedException();
        }
    }
}