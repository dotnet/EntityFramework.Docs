using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace SqlServer.Plugin;
internal class AugmentationTranslator : IMethodCallTranslator
{
    public SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
    {
        switch (method.Name)
        {
            case nameof(DbFunctionsExtensions.Augment):
                var argument = arguments[1];
                return new SqlBinaryExpression(ExpressionType.Add, argument, new SqlConstantExpression(Expression.Constant(1), null), argument.Type, null);
            default:
                throw new InvalidOperationException($"Unexpected method '{method.Name}' in '{nameof(RelationalDbFunctionsExtensions)}'.");
        }
    }
}
