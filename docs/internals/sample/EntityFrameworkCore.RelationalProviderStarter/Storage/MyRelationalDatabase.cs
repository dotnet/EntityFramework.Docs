using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;

namespace EntityFrameworkCore.RelationalProviderStarter.Storage
{
    public class MyRelationalDatabase : RelationalDatabase
    {
        public MyRelationalDatabase(IQueryCompilationContextFactory queryCompilationContextFactory,
            ICommandBatchPreparer batchPreparer,
            IBatchExecutor batchExecutor,
            IRelationalConnection connection)
            : base(queryCompilationContextFactory,
                batchPreparer,
                batchExecutor,
                connection)
        {
        }
    }
}