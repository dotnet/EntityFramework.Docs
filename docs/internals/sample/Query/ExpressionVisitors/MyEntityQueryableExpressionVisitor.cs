using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Remotion.Linq.Clauses;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using System;

namespace EntityFrameworkCore.ProviderStarter.Query.ExpressionVisitors
{
    internal class MyEntityQueryableExpressionVisitor : EntityQueryableExpressionVisitor
    {
        private EntityQueryModelVisitor queryModelVisitor;
        private IQuerySource querySource;

        public MyEntityQueryableExpressionVisitor(EntityQueryModelVisitor queryModelVisitor, IQuerySource querySource)
            : base(queryModelVisitor)
        {
            this.queryModelVisitor = queryModelVisitor;
            this.querySource = querySource;
        }

        protected override Expression VisitEntityQueryable(Type elementType)
        {
            throw new NotImplementedException();
        }
    }
}