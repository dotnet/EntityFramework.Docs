using System;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;

namespace EntityFrameworkCore.RelationalProviderStarter.Update
{
    public class MyUpdateSqlGenerator : UpdateSqlGenerator
    {
        public MyUpdateSqlGenerator(ISqlGenerationHelper sqlGenerationHelper)
            : base(sqlGenerationHelper)
        {
        }

        protected override void AppendIdentityWhereCondition(StringBuilder commandStringBuilder,
            ColumnModification columnModification)
        {
            throw new NotImplementedException();
        }

        protected override void AppendRowsAffectedWhereCondition(StringBuilder commandStringBuilder,
            int expectedRowsAffected)
        {
            throw new NotImplementedException();
        }
    }
}