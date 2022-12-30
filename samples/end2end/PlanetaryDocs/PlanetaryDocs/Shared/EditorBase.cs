using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using PlanetaryDocs.Domain;
using PlanetaryDocs.Services;

namespace PlanetaryDocs.Shared
{
    /// <summary>
    /// Code for <see cref="Editor"/> component.
    /// </summary>
    public class EditorBase : ComponentBase
    {
        private int changeCount;

        private bool ignoreParameterSet = false;

        private ValidationState aliasValidation =
                    ValidationRules.ValidResult();

        private ValidationState uidValidation =
            ValidationRules.ValidResult();

        private ValidationState titleValidation =
            ValidationRules.ValidResult();

        private ValidationState descriptionValidation =
            ValidationRules.ValidResult();

        private ValidationState markdownValidation =
            ValidationRules.ValidResult();

        /// <summary>
        /// Determines which element to set the focus for.
        /// </summary>
        protected enum FocusElement
        {
            /// <summary>
            /// No focus.
            /// </summary>
            None,

            /// <summary>
            /// Focus alias.
            /// </summary>
            Alias,

            /// <summary>
            /// Focus tag.
            /// </summary>
            Tag,
        }

        /// <summary>
        /// Gets or sets the <see cref="IDocumentService"/> implementation.
        /// </summary>
        [Inject]
        public IDocumentService DocumentService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="LoadingService"/>.
        /// </summary>
        [CascadingParameter]
        public LoadingService LoadingService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Document"/> to manipulate.
        /// </summary>
        [Parameter]
        public Document DocumentToEdit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the mode is edit or insert.
        /// </summary>
        [Parameter]
        public bool Insert { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Document"/> is
        /// ready and valid to insert or update.
        /// </summary>
        [Parameter]
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets the change notification for <see cref="IsValid"/>.
        /// </summary>
        [Parameter]
        public EventCallback<bool> IsValidChanged { get; set; }

        /// <summary>
        /// Gets or sets the count of detected changes.
        /// </summary>
        [Parameter]
        public int ChangeCount
        {
            get => changeCount;
            set
            {
                if (value != changeCount)
                {
                    changeCount = value;
                    InvokeAsync(async () => await ChangeCountChanged.InvokeAsync(changeCount));
                }
            }
        }

        /// <summary>
        /// Gets or sets the change notification for <see cref="ChangeCount"/>.
        /// </summary>
        [Parameter]
        public EventCallback<int> ChangeCountChanged { get; set; }

        /// <summary>
        /// Gets or sets the focus setting.
        /// </summary>
        protected FocusElement ElementToFocus { get; set; } = FocusElement.None;

        /// <summary>
        /// Gets or sets the <see cref="HtmlPreview"/> child element.
        /// </summary>
        protected HtmlPreview Preview { get; set; }

        /// <summary>
        /// Gets or sets the child <see cref="AliasSearch"/> component.
        /// </summary>
        protected AliasSearch AliasSearch { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the existing alias
        /// dialog.
        /// </summary>
        protected bool ExistingAlias { get; set; } = true;

        /// <summary>
        /// Gets or sets the <see cref="ValidatedInput"/> component for the
        /// new alias input.
        /// </summary>
        protected ValidatedInput NewAliasInput { get; set; }

        /// <summary>
        /// Gets or sets the list of tags.
        /// </summary>
        protected List<string> TagList
        {
            get => DocumentToEdit.Tags;
            set
            {
                DocumentToEdit.Tags.Clear();
                DocumentToEdit.Tags.AddRange(value);
                ChangeCount++;
                StateHasChanged();
            }
        }

        /// <summary>
        /// Gets or sets the title of the <see cref="Document"/>.
        /// </summary>
        protected string Title
        {
            get => DocumentToEdit.Title;
            set
            {
                if (value != DocumentToEdit.Title)
                {
                    DocumentToEdit.Title = value;
                    ChangeCount++;
                }
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier for the <see cref="Document"/>.
        /// </summary>
        protected string Uid
        {
            get => DocumentToEdit.Uid;
            set
            {
                if (value != DocumentToEdit.Uid)
                {
                    DocumentToEdit.Uid = value;
                    ChangeCount++;
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Document"/> description.
        /// </summary>
        protected string Description
        {
            get => DocumentToEdit.Description;
            set
            {
                if (value != DocumentToEdit.Description)
                {
                    DocumentToEdit.Description = value;
                    ChangeCount++;
                }
            }
        }

        /// <summary>
        /// Gets the list of validation states.
        /// </summary>
        protected List<ValidationState> ValidationStates { get; } =
            new List<ValidationState>();

        /// <summary>
        /// Gets or sets the markdown value.
        /// </summary>
        protected string Markdown
        {
            get => DocumentToEdit.Markdown;
            set
            {
                if (value != DocumentToEdit.Markdown)
                {
                    DocumentToEdit.Markdown = value;
                    MarkdownUpdated();
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Document"/> alias.
        /// </summary>
        protected string Alias
        {
            get => DocumentToEdit.AuthorAlias;
            set
            {
                if (value != DocumentToEdit.AuthorAlias)
                {
                    DocumentToEdit.AuthorAlias = value;
                    ChangeCount++;
                    InvokeAsync(ValidateAliasAsync);
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ValidationState"/> for <see cref="Title"/>.
        /// </summary>
        protected ValidationState TitleValidation
        {
            get => titleValidation;
            set
            {
                titleValidation = value;
                OnValidationChange();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ValidationState"/> for <see cref="Uid"/>.
        /// </summary>
        protected ValidationState UidValidation
        {
            get => uidValidation;
            set
            {
                uidValidation = value;
                OnValidationChange();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ValidationState"/> for <see cref="Markdown"/>.
        /// </summary>
        protected ValidationState MarkdownValidation
        {
            get => markdownValidation;
            set
            {
                markdownValidation = value;
                OnValidationChange();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ValidationState"/> for <see cref="Description"/>.
        /// </summary>
        protected ValidationState DescriptionValidation
        {
            get => descriptionValidation;
            set
            {
                descriptionValidation = value;
                OnValidationChange();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ValidationState"/> for <see cref="Alias"/>.
        /// </summary>
        protected ValidationState AliasValidation
        {
            get => aliasValidation;
            set
            {
                aliasValidation = value;
                OnValidationChange();
            }
        }

        /// <summary>
        /// Gets a value to display on the alias toggle.
        /// </summary>
        protected string AliasButton => ExistingAlias ? "Add New Alias"
            : "Choose Existing Alias";

        /// <summary>
        /// Gets or sets the HTML rendered from the <see cref="Markdown"/>.
        /// </summary>
        protected string Html { get; set; }

        /// <summary>
        /// Validate everything prior to committing.
        /// </summary>
        /// <param name="document">The version of the <see cref="Document"/> to validate.</param>
        /// <returns>A value indicating whether the document is valid.</returns>
        public bool ValidateAll(Document document)
        {
            var results = ValidationRules.ValidateDocument(document);
            var isValid = results.All(r => r.IsValid);
            if (isValid != IsValid)
            {
                ValidationStates.Clear();
                ValidationStates.AddRange(results.Where(r => !r.IsValid));
                ignoreParameterSet = true;
                OnValidationChange(false);
            }

            return isValid;
        }

        /// <summary>
        /// Calle when parameters are set.
        /// </summary>
        protected override void OnParametersSet()
        {
            if (ignoreParameterSet)
            {
                ignoreParameterSet = false;
                return;
            }

            Html = DocumentToEdit.Html;
            OnValidationChange();

            base.OnParametersSet();
        }

        /// <summary>
        /// Called after render.
        /// </summary>
        /// <param name="firstRender">A value indicating whether this is the first render.</param>
        /// <returns>An asynchronous task.</returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (ElementToFocus == FocusElement.Alias)
            {
                if (ExistingAlias)
                {
                    await AliasSearch.FocusAsync();
                }
                else
                {
                    await NewAliasInput.FocusAsync();
                }

                ElementToFocus = FocusElement.None;
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// Handle changes to validation state.
        /// </summary>
        /// <param name="resetList">A value indicating whether to rebuild the list of validations.</param>
        protected void OnValidationChange(bool resetList = true)
        {
            if (resetList)
            {
                ValidationStates.Clear();
                ValidationStates.AddRange(
                    new[]
                    {
                titleValidation,
                aliasValidation,
                descriptionValidation,
                markdownValidation,
                    });

                if (Insert)
                {
                    ValidationStates.Add(uidValidation);
                }
            }

            var isValid = ValidationStates.All(vr => vr.IsValid);

            if (isValid != IsValid)
            {
                IsValid = isValid;
                InvokeAsync(
                    async () =>
                    await IsValidChanged.InvokeAsync(isValid));
            }
        }

        /// <summary>
        /// Toggle the alias mode between choose existing and add new.
        /// </summary>
        /// <returns>An asynchronous task.</returns>
        protected async Task ToggleAliasAsync()
        {
            DocumentToEdit.AuthorAlias = string.Empty;
            ExistingAlias = !ExistingAlias;
            ElementToFocus = FocusElement.Alias;
            await ValidateAliasAsync();
        }

        /// <summary>
        /// Validates the alis value.
        /// </summary>
        /// <returns>An asynchronous task.</returns>
        protected async Task ValidateAliasAsync()
        {
            aliasValidation = ValidationRules.ValidateProperty(
                nameof(Document.AuthorAlias),
                ExistingAlias ? Alias : DocumentToEdit.AuthorAlias);

            if (!ExistingAlias && aliasValidation.IsValid)
            {
                var aliasToCheck = DocumentToEdit.AuthorAlias;
                List<string> aliases = null;

                await LoadingService.WrapExecutionAsync(
                    async () =>
                        aliases = await DocumentService.SearchAuthorsAsync(
                    aliasToCheck));
                if (aliases.Any(a => a == aliasToCheck))
                {
                    aliasValidation = new ValidationState
                    {
                        IsValid = false,
                        Message = $"Alias '{aliasToCheck}' already exists.",
                    };
                }
            }

            OnValidationChange();
        }

        /// <summary>
        /// Handle updates to <see cref="Markdown"/>.
        /// </summary>
        protected void MarkdownUpdated()
        {
            ChangeCount++;

            var checkRequired = ValidationRules.ValidateProperty(
                nameof(DocumentToEdit.Markdown), DocumentToEdit.Markdown);

            if (checkRequired.IsValid)
            {
                MarkdownValidation = ValidationRules.InvalidResult(
                        "Markdown has changed. Preview before saving.");
            }
            else if (markdownValidation.IsValid != checkRequired.IsValid ||
                    markdownValidation.Message != checkRequired.Message)
            {
                MarkdownValidation = checkRequired;
            }

            OnValidationChange();
        }

        /// <summary>
        /// Generate the HTML from markdown.
        /// </summary>
        protected void MarkdownPreview()
        {
            if (string.IsNullOrWhiteSpace(Markdown))
            {
                Html = string.Empty;
                DocumentToEdit.Html = string.Empty;
                InvokeAsync(Preview.OnUpdateAsync);
                return;
            }

            Html = Markdig.Markdown.ToHtml(DocumentToEdit.Markdown);
            DocumentToEdit.Html = Html;

            InvokeAsync(Preview.OnUpdateAsync);

            MarkdownValidation = ValidationRules.ValidResult();

            OnValidationChange();
        }
    }
}
