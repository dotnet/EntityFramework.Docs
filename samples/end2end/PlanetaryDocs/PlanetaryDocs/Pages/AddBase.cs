using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using PlanetaryDocs.Domain;
using PlanetaryDocs.Services;
using PlanetaryDocs.Shared;

namespace PlanetaryDocs.Pages
{
    /// <summary>
    /// Component to add a new document.
    /// </summary>
    public class AddBase : ComponentBase
    {
        private bool isValid = false;

        /// <summary>
        /// Gets or sets the navigation service.
        /// </summary>
        [Inject]
        public NavigationManager NavigationService { get; set; }

        /// <summary>
        /// Gets or sets the document service.
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
        /// Gets or sets a value indicating whether the <see cref="Document"/>
        /// is in a valid state for add.
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
        /// Gets a value indicating whether changes have been made.
        /// </summary>
        protected bool IsDirty => ChangeCount > 0;

        /// <summary>
        /// Gets or sets a value indicating whether a loading operation is taking place.
        /// </summary>
        protected bool Loading { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether a save operation is in effect.
        /// </summary>
        protected bool Saving { get; set; } = false;

        /// <summary>
        /// Gets or sets the <see cref="Document"/> being added.
        /// </summary>
        protected Document Document { get; set; } = new ();

        /// <summary>
        /// Gets or sets the count of changes detected.
        /// </summary>
        protected int ChangeCount { get; set; } = 0;

        /// <summary>
        /// Gets or sets the <see cref="Editor"/> reference.
        /// </summary>
        protected Editor Editor { get; set; }

        /// <summary>
        /// Main save method.
        /// </summary>
        /// <returns>An asynchronous task.</returns>
        public async Task SaveAsync()
        {
            if (!IsDirty || !IsValid || !Editor.ValidateAll(Document))
            {
                return;
            }

            Saving = true;

            await LoadingService.WrapExecutionAsync(async () =>
                await DocumentService.InsertDocumentAsync(Document));

            NavigationService.NavigateTo(NavigationHelper.ViewDocument(Document.Uid), true);
        }

        /// <summary>
        /// Set the title.
        /// </summary>
        /// <param name="firstRender">A flag indicating the render status.</param>
        /// <returns>The asynchronous task.</returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await TitleService.SetTitleAsync("Adding new document");
                Editor.ValidateAll(Document);
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// Initialize the component.
        /// </summary>
        protected override void OnInitialized()
        {
            Document = new Document();
            ChangeCount = 0;
            Loading = false;
            base.OnInitialized();
        }
    }
}
