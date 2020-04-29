using Items;
using Microsoft.EntityFrameworkCore;

namespace Tests
{
    public class InMemoryItemsControllerTest : ItemsControllerTest
    {
        public InMemoryItemsControllerTest()
            : base(
                new DbContextOptionsBuilder<ItemsContext>()
                    .UseInMemoryDatabase("TestDatabase")
                    .Options)
        {
        }
    }
}