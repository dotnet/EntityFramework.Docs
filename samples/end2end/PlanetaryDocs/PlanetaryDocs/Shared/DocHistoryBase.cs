using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using PlanetaryDocs.Domain;
using PlanetaryDocs.Services;

namespace PlanetaryDocs.Shared
{
    /// <summary>
    /// Code for the <see cref="DocHistory"/> component.
    /// </summary>
    public class DocHistoryBase : ComponentBase
    {
        private string lastUid = string.Empty;

        /// <summary>
        /// Gets or sets the <see cref="LoadingService"/>.
        /// </summary>
        [CascadingParameter]
        public LoadingService LoadingService { get; set; }

        /// <summary>
        /// Gets or sets the implementation of <see cref="IDocumentService"/>.
        /// </summary>
        [Inject]
        public IDocumentService DocumentService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="NavigationManager"/>.
        /// </summary>
        [Inject]
        public NavigationManager NavigationService { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the <see cref="Document"/> to get history for.
        /// </summary>
        [Parameter]
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the list of audit items that show the history of
        /// the <see cref="Document"/>.
        /// </summary>
        protected List<DocumentAuditSummary> History { get; set; } = null;

        /// <summary>
        /// Called when parameters change.
        /// </summary>
        /// <returns>The asynchronous test.</returns>
        protected override async Task OnParametersSetAsync()
        {
            if (Uid != lastUid)
            {
                lastUid = Uid;
                await LoadingService.WrapExecutionAsync(
                    async () =>
                        History =
                        await DocumentService.LoadDocumentHistoryAsync(lastUid));
            }

            await base.OnParametersSetAsync();
        }

        /// <summary>
        /// Navigate to a specific audit entry.
        /// </summary>
        /// <param name="audit">The <see cref="DocumentAuditSummary"/>.</param>
        protected void Navigate(DocumentAuditSummary audit) =>
            NavigationService.NavigateTo(
            NavigationHelper.ViewDocument(audit.Uid, audit.Id), true);
    }
}
