using System.Linq;
using Items;
using Items.Controllers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace SharedDatabaseTests
{
    #region UsingTheFixture
    public class SharedDatabaseTest : IClassFixture<SharedDatabaseFixture>
    {
        public SharedDatabaseTest(SharedDatabaseFixture fixture) => Fixture = fixture;

        public SharedDatabaseFixture Fixture { get; }
    
        #endregion
    
        #region CanGetItems
        [Fact]
        public void Can_get_items()
        {
            using (var context = Fixture.CreateContext())
            {
                var controller = new ItemsController(context);

                var items = controller.Get().ToList();
                
                Assert.Equal(3, items.Count);
                Assert.Equal("ItemOne", items[0].Name);
                Assert.Equal("ItemThree", items[1].Name);
                Assert.Equal("ItemTwo", items[2].Name);
            }
        }
        #endregion
        
        [Fact]
        public void Can_get_item()
        {
            using (var context = Fixture.CreateContext())
            {
                var controller = new ItemsController(context);

                var item = controller.Get("ItemTwo");
                
                Assert.Equal("ItemTwo", item.Name);
            }
        }
        
        #region CanAddItem
        [Fact]
        public void Can_add_item()
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
                {
                    var controller = new ItemsController(context);

                    var item = controller.PostItem("ItemFour").Value;

                    Assert.Equal("ItemFour", item.Name);
                }

                using (var context = Fixture.CreateContext(transaction))
                {
                    var item = context.Set<Item>().Single(e => e.Name == "ItemFour");

                    Assert.Equal("ItemFour", item.Name);
                    Assert.Equal(0, item.Tags.Count);
                }
            }
        }
        #endregion
        
        #region CanAddTag
        [Fact]
        public void Can_add_tag()
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
                {
                    var controller = new ItemsController(context);

                    var tag = controller.PostTag("ItemTwo", "Tag21").Value;

                    Assert.Equal("Tag21", tag.Label);
                    Assert.Equal(1, tag.Count);
                }

                using (var context = Fixture.CreateContext(transaction))
                {
                    var item = context.Set<Item>().Include(e => e.Tags).Single(e => e.Name == "ItemTwo");

                    Assert.Equal(1, item.Tags.Count);
                    Assert.Equal("Tag21", item.Tags[0].Label);
                    Assert.Equal(1, item.Tags[0].Count);
                }
            }
        }
        #endregion
        
        #region CanUpTagCount
        [Fact]
        public void Can_add_tag_when_already_existing_tag()
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
                {
                    var controller = new ItemsController(context);

                    var tag = controller.PostTag("ItemThree", "Tag32").Value;

                    Assert.Equal("Tag32", tag.Label);
                    Assert.Equal(3, tag.Count);
                }

                using (var context = Fixture.CreateContext(transaction))
                {
                    var item = context.Set<Item>().Include(e => e.Tags).Single(e => e.Name == "ItemThree");

                    Assert.Equal(2, item.Tags.Count);
                    Assert.Equal("Tag31", item.Tags[0].Label);
                    Assert.Equal(3, item.Tags[0].Count);
                    Assert.Equal("Tag32", item.Tags[1].Label);
                    Assert.Equal(3, item.Tags[1].Count);
                }
            }
        }
        #endregion
        
        #region DeleteItem
        [Fact]
        public void Can_remove_item_and_all_associated_tags()
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
                {
                    var controller = new ItemsController(context);

                    var item = controller.DeleteItem("ItemThree").Value;

                    Assert.Equal("ItemThree", item.Name);
                }

                using (var context = Fixture.CreateContext(transaction))
                {
                    Assert.False(context.Set<Item>().Any(e => e.Name == "ItemThree"));
                    Assert.False(context.Set<Tag>().Any(e => e.Label.StartsWith("Tag3")));
                }
            }
        }
        #endregion
    }
}
