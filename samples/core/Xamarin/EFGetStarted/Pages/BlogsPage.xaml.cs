using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using System.ComponentModel;
using System.Threading.Tasks;

namespace EFGetStarted
{
    public partial class BlogsPage : ContentPage, INotifyPropertyChanged
    {
        public BlogsPage()
        {
            InitializeComponent();                                 
        }

        protected async override void OnAppearing()
        {
            blobCollectionView.SelectedItem = null;

            using (var blogContext = new BloggingContext())
            {
                await InsertStartData(blogContext);

                var theBlogs = blogContext.Blogs.ToList();
                
                blobCollectionView.ItemsSource = theBlogs;
            }
        }

        async Task InsertStartData(BloggingContext context)
        {
            var blogCount = context.Blogs.Count();

            if (blogCount == 0)
            {
                await context.AddAsync(new Blog { Url = "https://devblogs.microsoft.com/xamarin" });
                await context.AddAsync(new Blog { Url = "https://aka.ms/appsonazureblog" });

                await context.SaveChangesAsync();
            }
        }

        async void blobCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(e.CurrentSelection.FirstOrDefault() is Blog blog))
                return;

            var postsPage = new PostsPage(blog.BlogId);
            await Navigation.PushAsync(postsPage);
        }

        async void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new AddBlogPage()));
        }

        async void DeleteAll_Clicked(object sender, EventArgs e)
        {
            using (var blogContext = new BloggingContext())
            {                
                blogContext.RemoveRange(blogContext.Blogs);

                await blogContext.SaveChangesAsync();
                
                blobCollectionView.ItemsSource = blogContext.Blogs.ToList();
            }
        }
    }
}
