using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Remotion.Linq.Clauses;

namespace EntityFrameworkCore.ProviderStarter.Query.ExpressionVisitors

{
    public class MyEntityQueryableExpressionVisitorFactory : IEntityQueryableExpressionVisitorFactory
    {
        public ExpressionVisitor Create(EntityQueryModelVisitor queryModelVisitor, IQuerySource querySource)
            => new MyEntityQueryableExpressionVisitor(queryModelVisitor, querySource);
    }
}