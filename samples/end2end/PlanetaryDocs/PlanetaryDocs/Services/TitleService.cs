using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace PlanetaryDocs.Services
{
    /// <summary>
    /// Service to set the browser title.
    /// </summary>
    public class TitleService
    {
        private const string DefaultTitle = "Planetary Docs";
        private readonly NavigationManager navigationManager;
        private readonly IJSRuntime jsRuntime;

        /// <summary>
        /// Initializes a new instance of the <see cref="TitleService"/> class.
        /// </summary>
        /// <param name="manager">The <see cref="NavigationManager"/>.</param>
        /// <param name="jsRuntime">The JavaScript runtime.</param>
        public TitleService(
            NavigationManager manager,
            IJSRuntime jsRuntime)
        {
            navigationManager = manager;
            navigationManager.LocationChanged += async (o, e) =>
                await SetTitleAsync(DefaultTitle);
            this.jsRuntime = jsRuntime;
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Main task to set the title.
        /// </summary>
        /// <param name="title">The title to use.</param>
        /// <returns>An asynchronous task.</returns>
        public async Task SetTitleAsync(string title)
        {
            Title = title;
            await jsRuntime.InvokeVoidAsync("titleService.setTitle", title);
        }
    }
}
