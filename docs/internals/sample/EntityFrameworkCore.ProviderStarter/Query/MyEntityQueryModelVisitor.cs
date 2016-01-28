using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace EntityFrameworkCore.ProviderStarter.Query
{
    public class MyEntityQueryModelVisitor : EntityQueryModelVisitor
    {
        public MyEntityQueryModelVisitor(
            IQueryOptimizer queryOptimizer,
            INavigationRewritingExpressionVisitorFactory navigationRewritingExpressionVisitorFactory,
            ISubQueryMemberPushDownExpressionVisitor subQueryMemberPushDownExpressionVisitor,
            IQuerySourceTracingExpressionVisitorFactory querySourceTracingExpressionVisitorFactory,
            IEntityResultFindingExpressionVisitorFactory entityResultFindingExpressionVisitorFactory,
            ITaskBlockingExpressionVisitor taskBlockingExpressionVisitor,
            IMemberAccessBindingExpressionVisitorFactory memberAccessBindingExpressionVisitorFactory,
            IOrderingExpressionVisitorFactory orderingExpressionVisitorFactory,
            IProjectionExpressionVisitorFactory projectionExpressionVisitorFactory,
            IEntityQueryableExpressionVisitorFactory entityQueryableExpressionVisitorFactory,
            IQueryAnnotationExtractor queryAnnotationExtractor,
            IResultOperatorHandler resultOperatorHandler,
            IEntityMaterializerSource entityMaterializerSource,
            IExpressionPrinter expressionPrinter,
            QueryCompilationContext queryCompilationContext)
            : base(queryOptimizer,
                navigationRewritingExpressionVisitorFactory,
                subQueryMemberPushDownExpressionVisitor,
                querySourceTracingExpressionVisitorFactory,
                entityResultFindingExpressionVisitorFactory,
                taskBlockingExpressionVisitor,
                memberAccessBindingExpressionVisitorFactory,
                orderingExpressionVisitorFactory,
                projectionExpressionVisitorFactory,
                entityQueryableExpressionVisitorFactory,
                queryAnnotationExtractor,
                resultOperatorHandler,
                entityMaterializerSource,
                expressionPrinter,
                queryCompilationContext)
        {
        }

        protected override void IncludeNavigations(
            IncludeSpecification includeSpecification,
            Type resultType,
            LambdaExpression accessorLambda,
            bool querySourceRequiresTracking)
        {
            throw new NotImplementedException("MyProvider does not support .Include()");
        }
    }
}