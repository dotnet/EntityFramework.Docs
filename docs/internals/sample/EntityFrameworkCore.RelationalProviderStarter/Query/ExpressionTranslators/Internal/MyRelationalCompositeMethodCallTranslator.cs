using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkCore.RelationalProviderStarter.Query.ExpressionTranslators.Internal
{
    public class MyRelationalCompositeMethodCallTranslator : RelationalCompositeMethodCallTranslator
    {
        public MyRelationalCompositeMethodCallTranslator(ILogger logger)
            : base(logger)
        {
        }
    }
}