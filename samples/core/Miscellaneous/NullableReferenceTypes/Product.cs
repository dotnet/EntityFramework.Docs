namespace NullableReferenceTypes
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Product(string name)
        {
            Name = name;
        }
    }
}
