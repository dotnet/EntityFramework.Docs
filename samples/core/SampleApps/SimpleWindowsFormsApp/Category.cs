using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SimpleWindowsFormsApp
{
    public class Category
    {
        private readonly ObservableCollectionListSource<Product> _products =
                new ObservableCollectionListSource<Product>();

        public int CategoryId { get; set; }
        public string Name { get; set; }
        public virtual ObservableCollectionListSource<Product> Products { get { return _products; } }
    }
}
