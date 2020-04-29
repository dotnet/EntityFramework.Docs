namespace Items
{
    #region TagEntityType
    public class Tag
    {
        private readonly int _id;

        private Tag(int id, string label)
        {
            _id = id;
            Label = label;
        }

        public Tag(string label) => Label = label;

        public string Label { get; }
        
        public int Count { get; set; }
    }
    #endregion
}