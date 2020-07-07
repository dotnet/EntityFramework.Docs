using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace EFGetStarted
{
    public partial class AddBlogPage : ContentPage
    {
        public AddBlogPage()
        {
            InitializeComponent();
        }

        async void Save_Clicked(System.Object sender, System.EventArgs e)
        {
            var blog = new Blog { Url = blogUrl.Text };

            using (var blogContext = new BloggingContext())
            {
                blogContext.Add(blog);

                await blogContext.SaveChangesAsync();
            }

            await Navigation.PopModalAsync();
        }

        async void Cancel_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
