using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Xamarin.Forms;

namespace EFGetStarted
{
    public partial class AddPostPage : ContentPage
    {
        int BlogId;

        public AddPostPage(int blogId)
        {
            InitializeComponent();

            BlogId = blogId;
        }

        protected async void Save_Clicked(object sender, EventArgs e)
        {
            var newPost = new Post
            {
                BlogId = BlogId,
                Content = postCell.Text,
                Title = titleCell.Text
            };

            try
            {
                using (var blogContext = new BloggingContext())
                {
                    await blogContext.Posts.AddAsync(newPost);

                    await blogContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

            await Navigation.PopModalAsync();
        }

        protected async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
