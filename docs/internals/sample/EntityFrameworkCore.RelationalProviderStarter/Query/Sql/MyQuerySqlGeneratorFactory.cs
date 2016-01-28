using System;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.RelationalProviderStarter.Query.Sql
{
    public class MyQuerySqlGeneratorFactory : QuerySqlGeneratorFactoryBase
    {
        public MyQuerySqlGeneratorFactory(
            IRelationalCommandBuilderFactory commandBuilderFactory,
            ISqlGenerationHelper sqlGenerationHelper,
            IParameterNameGeneratorFactory parameterNameGeneratorFactory,
            IRelationalTypeMapper relationalTypeMapper)
            : base(
                commandBuilderFactory,
                sqlGenerationHelper,
                parameterNameGeneratorFactory,
                relationalTypeMapper)
        {
        }

        public override IQuerySqlGenerator CreateDefault(SelectExpression selectExpression)
        {
            throw new NotImplementedException();
        }
    }
}