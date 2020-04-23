using System.Linq;
using Items;
using Items.Controllers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tests
{
    public abstract class ItemsControllerTest
    {
        #region Seeding
        protected ItemsControllerTest(DbContextOptions<ItemsContext> contextOptions)
        {
            ContextOptions = contextOptions;
            
            Seed();
        }

        protected DbContextOptions<ItemsContext> ContextOptions { get; }

        private void Seed()
        {
            using (var context = new ItemsContext(ContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                
                var one = new Item("ItemOne");
                one.AddTag("Tag11");
                one.AddTag("Tag12");
                one.AddTag("Tag13");
                
                var two = new Item("ItemTwo");
                
                var three = new Item("ItemThree");
                three.AddTag("Tag31");
                three.AddTag("Tag31");
                three.AddTag("Tag31");
                three.AddTag("Tag32");
                three.AddTag("Tag32");
                
                context.AddRange(one, two, three);
                
                context.SaveChanges();
            }
        }
        #endregion

        #region CanGetItems
        [Fact]
        public void Can_get_items()
        {
            using (var context = new ItemsContext(ContextOptions))
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
            using (var context = new ItemsContext(ContextOptions))
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
            using (var context = new ItemsContext(ContextOptions))
            {
                var controller = new ItemsController(context);

                var item = controller.PostItem("ItemFour").Value;
                
                Assert.Equal("ItemFour", item.Name);
            }
            
            using (var context = new ItemsContext(ContextOptions))
            {
                var item = context.Set<Item>().Single(e => e.Name == "ItemFour");
                
                Assert.Equal("ItemFour", item.Name);
                Assert.Equal(0, item.Tags.Count);
            }
        }
        #endregion
        
        #region CanAddItemCaseInsensitive
        [Fact]
        public void Can_add_item_differing_only_by_case()
        {
            using (var context = new ItemsContext(ContextOptions))
            {
                var controller = new ItemsController(context);

                var item = controller.PostItem("itemtwo").Value;
                
                Assert.Equal("itemtwo", item.Name);
            }
            
            using (var context = new ItemsContext(ContextOptions))
            {
                var item = context.Set<Item>().Single(e => e.Name == "itemtwo");
                
                Assert.Equal(0, item.Tags.Count);
            }
        }
        #endregion

        #region CanAddTag
        [Fact]
        public void Can_add_tag()
        {
            using (var context = new ItemsContext(ContextOptions))
            {
                var controller = new ItemsController(context);

                var tag = controller.PostTag("ItemTwo", "Tag21").Value;
                
                Assert.Equal("Tag21", tag.Label);
                Assert.Equal(1, tag.Count);
            }
            
            using (var context = new ItemsContext(ContextOptions))
            {
                var item = context.Set<Item>().Include(e => e.Tags).Single(e => e.Name == "ItemTwo");
                
                Assert.Equal(1, item.Tags.Count);
                Assert.Equal("Tag21", item.Tags[0].Label);
                Assert.Equal(1, item.Tags[0].Count);
            }
        }
        #endregion
        
        #region CanUpTagCount
        [Fact]
        public void Can_add_tag_when_already_existing_tag()
        {
            using (var context = new ItemsContext(ContextOptions))
            {
                var controller = new ItemsController(context);

                var tag = controller.PostTag("ItemThree", "Tag32").Value;
                
                Assert.Equal("Tag32", tag.Label);
                Assert.Equal(3, tag.Count);
            }
            
            using (var context = new ItemsContext(ContextOptions))
            {
                var item = context.Set<Item>().Include(e => e.Tags).Single(e => e.Name == "ItemThree");
                
                Assert.Equal(2, item.Tags.Count);
                Assert.Equal("Tag31", item.Tags[0].Label);
                Assert.Equal(3, item.Tags[0].Count);
                Assert.Equal("Tag32", item.Tags[1].Label);
                Assert.Equal(3, item.Tags[1].Count);
            }
        }
        #endregion
        
        #region DeleteItem
        [Fact]
        public void Can_remove_item_and_all_associated_tags()
        {
            using (var context = new ItemsContext(ContextOptions))
            {
                var controller = new ItemsController(context);

                var item = controller.DeleteItem("ItemThree").Value;
                
                Assert.Equal("ItemThree", item.Name);
            }
            
            using (var context = new ItemsContext(ContextOptions))
            {
                Assert.False(context.Set<Item>().Any(e => e.Name == "ItemThree"));
                Assert.False(context.Set<Tag>().Any(e => e.Label.StartsWith("Tag3")));
            }
        }
        #endregion
    }
}
