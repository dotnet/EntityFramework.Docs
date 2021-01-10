using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace GetStartedWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ProductContext _context =
            new ProductContext();

        private CollectionViewSource categoryViewSource;

        public MainWindow()
        {
            InitializeComponent();
            categoryViewSource =
                (CollectionViewSource)FindResource(nameof(categoryViewSource));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // this is for demo purposes only, to make it easier
            // to get up and running
            _context.Database.EnsureCreated();

            // load the entities into EF Core
            _context.Categories.Load();

            // bind to the source
            categoryViewSource.Source =
                _context.Categories.Local.ToObservableCollection();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // all changes are automatically tracked, including
            // deletes!
            _context.SaveChanges();

            // this forces the grid to refresh to latest values
            categoryDataGrid.Items.Refresh();
            productsDataGrid.Items.Refresh();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // clean up database connections
            _context.Dispose();
            base.OnClosing(e);
        }
    }
}
