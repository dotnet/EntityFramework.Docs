using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Items.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        #region Constructor
        private readonly ItemsContext _context;

        public ItemsController(ItemsContext context) 
            => _context = context;
        #endregion

        #region Get
        [HttpGet]
        public IEnumerable<Item> Get() 
            => _context.Set<Item>().Include(e => e.Tags).OrderBy(e => e.Name);

        [HttpGet]
        public Item Get(string itemName) 
            => _context.Set<Item>().Include(e => e.Tags).FirstOrDefault(e => e.Name == itemName);
        #endregion

        #region PostItem
        [HttpPost]
        public ActionResult<Item> PostItem(string itemName)
        {
            var item = _context.Add(new Item(itemName)).Entity;

            _context.SaveChanges();
            
            return item;
        }
        #endregion

        #region PostTag
        [HttpPost]
        public ActionResult<Tag> PostTag(string itemName, string tagLabel)
        {
            var tag = _context
                .Set<Item>()
                .Include(e => e.Tags)
                .Single(e => e.Name == itemName)
                .AddTag(tagLabel);

            _context.SaveChanges();
            
            return tag;
        }
        #endregion
        
        #region DeleteItem
        [HttpDelete("{itemName}")]
        public ActionResult<Item> DeleteItem(string itemName)
        {
            var item = _context
                .Set<Item>()
                .SingleOrDefault(e => e.Name == itemName);

            if (item == null)
            {
                return NotFound();
            }

            _context.Remove(item);
            _context.SaveChanges();

            return item;
        }
        #endregion
    }
}
