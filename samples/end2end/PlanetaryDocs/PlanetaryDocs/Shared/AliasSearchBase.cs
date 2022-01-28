using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using PlanetaryDocs.Domain;
using PlanetaryDocs.Services;

namespace PlanetaryDocs.Shared
{
    /// <summary>
    /// Base code for the <see cref="AliasSearch"/> component.
    /// </summary>
    public class AliasSearchBase : ComponentBase
    {
        private string alias;

        /// <summary>
        /// Gets or sets the <see cref="LoadingService"/>.
        /// </summary>
        [CascadingParameter]
        public LoadingService LoadingService { get; set; }

        /// <summary>
        /// Gets or sets the implementation of the <see cref="IDocumentService"/>.
        /// </summary>
        [Inject]
        public IDocumentService DocumentService { get; set; }

        /// <summary>
        /// Gets or sets the tab index for keyboard navigation.
        /// </summary>
        [Parameter]
        public string TabIndex { get; set; }

        /// <summary>
        /// Gets or sets the alias selected.
        /// </summary>
        [Parameter]
        public string Alias
        {
            get => alias;
            set
            {
                if (value != alias)
                {
                    alias = value;
                    InvokeAsync(async () => await AliasChanged.InvokeAsync(alias));
                }
            }
        }

        /// <summary>
        /// Gets or sets the callback to notify on alias selection changes.
        /// </summary>
        [Parameter]
        public EventCallback<string> AliasChanged { get; set; }

        /// <summary>
        /// Gets or sets the reference to the child
        /// <see cref="AutoComplete"/> component.
        /// </summary>
        protected AutoComplete AutoComplete { get; set; }

        /// <summary>
        /// Search alias based on the text passed in.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <returns>The list of matching aliases.</returns>
        public async Task<List<string>> SearchAsync(string searchText)
        {
            List<string> results = null;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                return results;
            }

            await LoadingService.WrapExecutionAsync(
                async () => results =
                await DocumentService.SearchAuthorsAsync(searchText));

            return results;
        }

        /// <summary>
        /// Method to set focus to component.
        /// </summary>
        /// <returns>The asynchronous task.</returns>
        public async Task FocusAsync() => await AutoComplete.FocusAsync();
    }
}
