using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using PlanetaryDocs.Domain;
using PlanetaryDocs.Services;

namespace PlanetaryDocs.Pages
{
    /// <summary>
    /// Code for the view component.
    /// </summary>
    public class ViewBase : ComponentBase
    {
        private string uid = string.Empty;

        /// <summary>
        /// Gets or sets the implementation of <see cref="IDocumentService"/>.
        /// </summary>
        [Inject]
        public IDocumentService DocumentService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="LoadingService"/>.
        /// </summary>
        [CascadingParameter]
        public LoadingService LoadingService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="NavigationManager"/>.
        /// </summary>
        [Inject]
        public NavigationManager NavigationService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TitleService"/>.
        /// </summary>
        [Inject]
        public TitleService TitleService { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the documnt.
        /// </summary>
        [Parameter]
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the document history.
        /// </summary>
        protected bool ShowHistory { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to show the "Preview HTML" option.
        /// </summary>
        protected bool PreviewHtml { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to show the option to switch to Markdown.
        /// </summary>
        protected bool ShowMarkdown { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether a loading operation is happening.
        /// </summary>
        protected bool Loading { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the document exists.
        /// </summary>
        protected bool NotFound { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to show recent audit history.
        /// </summary>
        protected bool Audit { get; set; } = false;

        /// <summary>
        /// Gets or sets the <see cref="Document"/> to view.
        /// </summary>
        protected Document Document { get; set; } = null;

        /// <summary>
        /// Gets the text to toggle between markdown and preiew or HTML.
        /// </summary>
        protected string ToggleText => ShowMarkdown ?
            "Show HTML" : "Show Markdown";

        /// <summary>
        /// Gets the text to show preview text or rendered HTML.
        /// </summary>
        protected string PreviewText => PreviewHtml ?
            "Show Source" : "Show Preview";

        /// <summary>
        /// Gets the title for the current item.
        /// </summary>
        protected string Title => Audit ? $"[ARCHIVE] {Document?.Title}"
            : Document?.Title;

        /// <summary>
        /// Called when the <see cref="Document"/> identifier is set.
        /// </summary>
        /// <returns>An asynchronous task.</returns>
        protected override async Task OnParametersSetAsync()
        {
            var newUid = WebUtility.UrlDecode(Uid);
            if (newUid != uid)
            {
                var history = string.Empty;
                var query = NavigationHelper.GetQueryString(
                    NavigationService.Uri);
                if (query.ContainsKey(nameof(history)))
                {
                    history = query[nameof(history)];
                }

                Loading = false;
                NotFound = false;
                uid = newUid;
                try
                {
                    Loading = true;
                    if (string.IsNullOrWhiteSpace(history))
                    {
                        await LoadingService.WrapExecutionAsync(
                            async () => Document = await
                                DocumentService.LoadDocumentAsync(uid));
                        NotFound = Document == null;
                        Audit = false;
                    }
                    else
                    {
                        await LoadingService.WrapExecutionAsync(
                            async () => Document = await
                                DocumentService.LoadDocumentSnapshotAsync(Guid.Parse(history), uid));
                        Audit = true;
                        NotFound = Document == null;
                    }

                    Loading = false;
                    await TitleService.SetTitleAsync($"Viewing {Title}");
                }
                catch
                {
                    NotFound = true;
                }
            }

            await base.OnParametersSetAsync();
        }

        /// <summary>
        /// Go back to main version of the document.
        /// </summary>
        protected void BackToMain() =>
            NavigationService.NavigateTo(
                NavigationHelper.ViewDocument(Uid),
                true);
    }
}
