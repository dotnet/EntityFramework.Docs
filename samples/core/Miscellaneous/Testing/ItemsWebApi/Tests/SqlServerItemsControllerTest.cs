using Items;
using Microsoft.EntityFrameworkCore;

namespace Tests
{
    public class SqlServerItemsControllerTest : ItemsControllerTest
    {
        public SqlServerItemsControllerTest()
            : base(
                new DbContextOptionsBuilder<ItemsContext>()
                    .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFTestSample;ConnectRetryCount=0")
                    .Options)
        {
        }
    }
}