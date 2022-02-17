using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace PlanetaryDocs.Shared
{
    /// <summary>
    /// Code for the HTML preview component.
    /// </summary>
    public class HtmlPreviewBase : ComponentBase
    {
        private bool rendered;
        private string html;

        /// <summary>
        /// Gets or sets a value indicating whether the preview is shown in an
        /// edit context.
        /// </summary>
        [Parameter]
        public bool IsEdit { get; set; }

        /// <summary>
        /// Gets or sets the HTML to preview.
        /// </summary>
        [Parameter]
        public string Html
        {
            get => html;
            set
            {
                if (value != html)
                {
                    html = value;
                    HtmlToRender = new MarkupString(html);
                }
            }
        }

        /// <summary>
        /// Gets the HTML to render.
        /// </summary>
        protected MarkupString HtmlToRender { get; private set; }

        /// <summary>
        /// Gets the CSS class to use based on the mode.
        /// </summary>
        protected string WebClass => IsEdit ? "webedit" : "web";

        /// <summary>
        /// Method to update the preview.
        /// </summary>
        /// <remarks>
        /// This component uses a "trick" to render HTML by inserting it into a
        /// <c>textarea</c> element then moving the value over. The code is in
        /// <c>wwwroot/js/markdownExtensions.js</c>.
        /// </remarks>
        /// <returns>The asynchronous task.</returns>
        public async Task OnUpdateAsync()
        {
            if (rendered)
            {
                await InvokeAsync(() => HtmlToRender = new MarkupString(Html));
            }
        }

        /// <summary>
        /// Called after rendering.
        /// </summary>
        /// <param name="firstRender">A value indicating whether it is the first render.</param>
        /// <returns>The asynchronous task.</returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                rendered = true;
                await OnUpdateAsync();
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
