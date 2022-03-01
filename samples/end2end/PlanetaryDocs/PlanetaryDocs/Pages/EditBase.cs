using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using PlanetaryDocs.Domain;
using PlanetaryDocs.Services;
using PlanetaryDocs.Shared;

namespace PlanetaryDocs.Pages
{
    /// <summary>
    /// Base for edit component.
    /// </summary>
    public class EditBase : ComponentBase
    {
        private string uid = string.Empty;
        private bool isValid = false;

        /// <summary>
        /// Gets or sets the <see cref="NavigationManager"/>.
        /// </summary>
        [Inject]
        public NavigationManager NavigationService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDocumentService"/> implementation.
        /// </summary>
        [Inject]
        public IDocumentService DocumentService { get; set; }

        /// <summary>
        /// Gets or sets the loading service.
        /// </summary>
        [CascadingParameter]
        public LoadingService LoadingService { get; set; }

        /// <summary>
        /// Gets or sets the title service.
        /// </summary>
        [Inject]
        public TitleService TitleService { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the document being edited.
        /// </summary>
        [Parameter]
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Document"/>
        /// is valid for an update.
        /// </summary>
        public bool IsValid
        {
            get => isValid;
            set
            {
                if (value != isValid)
                {
                    isValid = value;
                    InvokeAsync(StateHasChanged);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not changes have been made.
        /// </summary>
        protected bool IsDirty => ChangeCount > 0;

        /// <summary>
        /// Gets or sets the count of detected changes.
        /// </summary>
        protected int ChangeCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Document"/>
        /// could not be found.
        /// </summary>
        protected bool NotFound { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a concurrency error was
        /// encountered during the last update attempt.
        /// </summary>
        protected bool Concurrency { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether items are being loaded.
        /// </summary>
        protected bool Loading { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Document"/>
        /// is being saved.
        /// </summary>
        protected bool Saving { get; set; }

        /// <summary>
        /// Gets or sets the Document being edited.
        /// </summary>
        protected Document Document { get; set; }

        /// <summary>
        /// Gets or sets a reference to the <see cref="Editor"/> child component.
        /// </summary>
        protected Editor Editor { get; set; }

        /// <summary>
        /// Save operation.
        /// </summary>
        /// <returns>An asynchronous task.</returns>
        public async Task SaveAsync()
        {
            if (!IsDirty || !IsValid || !Editor.ValidateAll(Document))
            {
                return;
            }

            Saving = true;

            if (Concurrency)
            {
                Concurrency = false;
                Document original = null;

                await LoadingService.WrapExecutionAsync(async () =>
                    original =
                    await DocumentService.LoadDocumentAsync(Document.Uid));

                Document.ETag = original.ETag;
            }

            try
            {
                await LoadingService.WrapExecutionAsync(async () =>
                    await DocumentService.UpdateDocumentAsync(Document));
            }
            catch (DbUpdateConcurrencyException)
            {
                Concurrency = true;
            }

            if (!Concurrency)
            {
                NavigationService.NavigateTo(NavigationHelper.ViewDocument(Document.Uid), true);
            }
            else
            {
                Saving = false;
            }
        }

        /// <summary>
        /// Called after render to set the title.
        /// </summary>
        /// <param name="firstRender">A value indicating whether this is the first render.</param>
        /// <returns>An asynchronous task.</returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && !string.IsNullOrWhiteSpace(Uid))
            {
                await TitleService.SetTitleAsync($"Editing '{Uid}'");
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// Called when the parameters are set. Triggers the document load.
        /// </summary>
        /// <returns>An asynchronous task.</returns>
        protected override async Task OnParametersSetAsync()
        {
            if (Uid != uid)
            {
                uid = Uid;
                Loading = true;
                Document = null;
                Concurrency = false;

                await LoadingService.WrapExecutionAsync(
                    async () => Document =
                    await DocumentService.LoadDocumentAsync(Uid));

                NotFound = Document == null;
                ChangeCount = 0;
                Loading = false;
            }

            await base.OnParametersSetAsync();
        }
    }
}
