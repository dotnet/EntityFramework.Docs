using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using PlanetaryDocs.Domain;
using PlanetaryDocs.Services;

namespace PlanetaryDocs.Pages
{
    /// <summary>
    /// Code for the index component.
    /// </summary>
    public class IndexBase : ComponentBase
    {
        private bool navigatingToThisPage = true;
        private bool searchQueued = false;

        /// <summary>
        /// Gets or sets a value indicating whether an asynchronous loading
        /// operation is taking place.
        /// </summary>
        protected bool Loading { get; set; } = true;

        /// <summary>
        /// Gets or sets the alias to filter search.
        /// </summary>
        protected string Alias { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the tag to filter search.
        /// </summary>
        protected string Tag { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the text to filter search.
        /// </summary>
        protected string Text { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the search results.
        /// </summary>
        protected List<DocumentSummary> DocsList { get; set; } = null;

        /// <summary>
        /// Gets or sets the reference to the text input element for focus.
        /// </summary>
        protected ElementReference InputElement { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="NavigationManager"/>.
        /// </summary>
        [Inject]
        protected NavigationManager NavigationService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDocumentService"/> implementation.
        /// </summary>
        [Inject]
        protected IDocumentService DocumentService { get; set; }

        /// <summary>
        /// Gets or sets the loading service.
        /// </summary>
        [CascadingParameter]
        protected LoadingService LoadingService { get; set; }

        /// <summary>
        /// Gets a value indicating whether there is enough information to
        /// perform a search.
        /// </summary>
        protected bool CanSearch =>
            !Loading && (
                (!string.IsNullOrWhiteSpace(Text) &&
                    Text.Trim().Length > 2)
                || !string.IsNullOrWhiteSpace(Alias)
                || !string.IsNullOrWhiteSpace(Tag));

        /// <summary>
        /// Handle parsing search values passed in via query string.
        /// </summary>
        /// <param name="firstRender">A value indicating whether this is the
        /// first render.</param>
        protected override void OnAfterRender(bool firstRender)
        {
            var stateHasChanged = false;

            if (navigatingToThisPage)
            {
                Loading = false;
                stateHasChanged = true;
                var queryValues = NavigationHelper.GetQueryString(
                    NavigationService.Uri);

                var hasSearch = false;

                foreach (var key in queryValues.Keys)
                {
                    switch (key)
                    {
                        case nameof(Text):
                            Text = queryValues[key];
                            hasSearch = true;
                            break;
                        case nameof(Alias):
                            Alias = queryValues[key];
                            hasSearch = true;
                            break;
                        case nameof(Tag):
                            Tag = queryValues[key];
                            hasSearch = true;
                            break;
                    }
                }

                navigatingToThisPage = false;
                if (hasSearch)
                {
                    InvokeAsync(async () => await SearchAsync());
                }
            }

            if (stateHasChanged)
            {
                StateHasChanged();
            }

            base.OnAfterRender(firstRender);
        }

        /// <summary>
        /// Handle keyboad events, i.e. Enter key to submit.
        /// </summary>
        /// <param name="key">The key pressed event arguments.</param>
        protected void HandleKeyPress(KeyboardEventArgs key)
        {
            if (key.Key == KeyNames.Enter)
            {
                InvokeAsync(SearchAsync);
            }
        }

        /// <summary>
        /// Search function.
        /// </summary>
        /// <returns>An asynchronous task.</returns>
        protected async Task SearchAsync()
        {
            if (Loading)
            {
                searchQueued = true;
                return;
            }

            searchQueued = false;

            Alias = Alias.Trim();
            Tag = Tag.Trim();
            Text = Text.Trim();

            if (CanSearch)
            {
                Loading = true;

                do
                {
                    searchQueued = false;
                    await LoadingService.WrapExecutionAsync(
                        async () =>
                    DocsList = await DocumentService.QueryDocumentsAsync(
                        Text,
                        Alias,
                        Tag));
                }
                while (searchQueued);

                Loading = false;
            }
            else
            {
                DocsList = null;
            }

            StateHasChanged();

            var queryString =
                NavigationHelper.CreateQueryString(
                    (nameof(Text), WebUtility.UrlEncode(Text)),
                    (nameof(Alias), WebUtility.UrlEncode(Alias)),
                    (nameof(Tag), WebUtility.UrlEncode(Tag)));

            navigatingToThisPage = false;
            NavigationService.NavigateTo($"/?{queryString}");

            await InputElement.FocusAsync();
        }

        /// <summary>
        /// Navigate to view a particular uid.
        /// </summary>
        /// <param name="uid">The unique document identifier.</param>
        protected void Navigate(string uid)
        {
            navigatingToThisPage = false;
            NavigationService.NavigateTo(
                NavigationHelper.ViewDocument(uid));
        }
    }
}
