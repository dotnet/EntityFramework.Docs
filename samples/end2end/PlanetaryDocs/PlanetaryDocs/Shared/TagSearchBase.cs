using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using PlanetaryDocs.Domain;
using PlanetaryDocs.Services;

namespace PlanetaryDocs.Shared
{
    /// <summary>
    /// Code for the <see cref="TagSearch"/> component.
    /// </summary>
    public class TagSearchBase : ComponentBase
    {
        private string tag;

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
        /// Gets or sets the tab index.
        /// </summary>
        [Parameter]
        public string TabIndex { get; set; }

        /// <summary>
        /// Gets or sets the selected tag.
        /// </summary>
        [Parameter]
        public string Tag
        {
            get => tag;
            set
            {
                if (value != tag)
                {
                    tag = value;
                    InvokeAsync(
                        async () =>
                            await TagChanged.InvokeAsync(tag));
                }
            }
        }

        /// <summary>
        /// Gets or sets the callback to notify on tag changes.
        /// </summary>
        [Parameter]
        public EventCallback<string> TagChanged { get; set; }

        /// <summary>
        /// Call the search and obtain results.
        /// </summary>
        /// <param name="searchText">The text to search.</param>
        /// <returns>The list of results.</returns>
        public async Task<List<string>> SearchAsync(string searchText)
        {
            List<string> results = null;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                return results;
            }

            await LoadingService.WrapExecutionAsync(
                async () => results =
                await DocumentService.SearchTagsAsync(searchText));

            return results;
        }
    }
}
