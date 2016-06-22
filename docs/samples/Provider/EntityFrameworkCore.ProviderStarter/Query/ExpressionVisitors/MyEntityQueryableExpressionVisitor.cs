using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Remotion.Linq.Clauses;

namespace EntityFrameworkCore.ProviderStarter.Query.ExpressionVisitors
{
    internal class MyEntityQueryableExpressionVisitor : EntityQueryableExpressionVisitor
    {
        private EntityQueryModelVisitor _queryModelVisitor;
        private IQuerySource _querySource;

        public MyEntityQueryableExpressionVisitor(EntityQueryModelVisitor queryModelVisitor, IQuerySource querySource)
            : base(queryModelVisitor)
        {
            _queryModelVisitor = queryModelVisitor;
            _querySource = querySource;
        }

        protected override Expression VisitEntityQueryable(Type elementType)
        {
            throw new NotImplementedException();
        }
    }
}