using System;
using System.Collections.Generic;
using System.Linq;

namespace Items
{
    #region ItemEntityType
    public class Item
    {
        private readonly int _id;
        private readonly List<Tag> _tags = new List<Tag>();

        private Item(int id, string name)
        {
            _id = id;
            Name = name;
        }

        public Item(string name)
        {
            Name = name;
        }

        public Tag AddTag(string label)
        {
            var tag = _tags.FirstOrDefault(t => t.Label == label);

            if (tag == null)
            {
                tag = new Tag(label);
                _tags.Add(tag);
            }
            
            tag.Count++;

            return tag;
        }

        public string Name { get; }
        
        public IReadOnlyList<Tag> Tags => _tags;
    }
    #endregion
}