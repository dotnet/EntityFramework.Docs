using System.Collections.ObjectModel;

namespace GetStartedWPF
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }

        public virtual ObservableCollection<Product>
            Products
        { get; private set; } =
            new ObservableCollection<Product>();
    }
}
