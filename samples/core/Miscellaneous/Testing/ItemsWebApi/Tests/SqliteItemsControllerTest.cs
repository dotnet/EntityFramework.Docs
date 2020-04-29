using Items;
using Microsoft.EntityFrameworkCore;

namespace Tests
{
    #region SqliteItemsControllerTest
    public class SqliteItemsControllerTest : ItemsControllerTest
    {
        public SqliteItemsControllerTest()
            : base(
                new DbContextOptionsBuilder<ItemsContext>()
                    .UseSqlite("Filename=Test.db")
                    .Options)
        {
        }
    }
    #endregion
}