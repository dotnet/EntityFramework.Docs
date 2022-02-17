using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using PlanetaryDocs.Services;

namespace PlanetaryDocs.Shared
{
    /// <summary>
    /// Code for the <see cref="EditBar"/> component.
    /// </summary>
    public class EditBarBase : ComponentBase
    {
        /// <summary>
        /// Gets or sets the <see cref="NavigationService"/>.
        /// </summary>
        [Inject]
        public NavigationManager NavigationService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="HistoryService"/>.
        /// </summary>
        [Inject]
        public HistoryService HistoryService { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the document
        /// has been modified.
        /// </summary>
        [Parameter]
        public bool IsDirty { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the document is in a valid
        /// state to update or insert.
        /// </summary>
        [Parameter]
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets the count of changes that are detected.
        /// </summary>
        [Parameter]
        public int ChangeCount { get; set; }

        /// <summary>
        /// Gets or sets the method to call to commit (save) changes.
        /// </summary>
        [Parameter]
        public Func<Task> SaveAsync { get; set; }

        /// <summary>
        /// Reset method returns to pre-edited state.
        /// </summary>
        protected void Reset() => NavigationService.NavigateTo(
NavigationService.Uri,
true);

        /// <summary>
        /// Cancel goes back to previous page.
        /// </summary>
        protected void Cancel() => InvokeAsync(async () =>
                     await HistoryService.GoBackAsync());
    }
}
