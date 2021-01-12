using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;

namespace EFGetStarted
{
    public partial class PostsPage : ContentPage
    {
        int BlogId;

        public PostsPage(int blogId)
        {
            InitializeComponent();

            BlogId = blogId;
        }

        protected override void OnAppearing()
        {
            using (var blogContext = new BloggingContext())
            {
                var postList = blogContext.Posts
                    .Where(p => p.BlogId == BlogId)
                    .ToList();

                postCollection.ItemsSource = postList;
            }
        }

        async void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new AddPostPage(BlogId)));
        }
    }
}
