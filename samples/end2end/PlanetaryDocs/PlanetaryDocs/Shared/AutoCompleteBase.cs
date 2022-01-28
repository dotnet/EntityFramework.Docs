using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using PlanetaryDocs.Services;

namespace PlanetaryDocs.Shared
{
    /// <summary>
    /// Code for the <see cref="AutoComplete"/> component.
    /// </summary>
    public class AutoCompleteBase : ComponentBase
    {
        private int index = -1;
        private bool loading = false;
        private bool queued = false;
        private string val = string.Empty;
        private string tabIndex;

        /// <summary>
        /// Gets or sets the tab index.
        /// </summary>
        [Parameter]
        public string TabIndex
        {
            get => tabIndex;
            set
            {
                tabIndex = value;
                if (int.TryParse(value, out var numIndex))
                {
                    BaseIndex = numIndex;
                }
            }
        }

        /// <summary>
        /// Gets or sets the current search value.
        /// </summary>
        public string Value
        {
            get => val;
            set
            {
                if (val != value)
                {
                    val = value;
                    InvokeAsync(OnValueChangedAsync);
                }
            }
        }

        /// <summary>
        /// Gets or sets the text to use as a label.
        /// </summary>
        [Parameter]
        public string LabelText { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the placeholder text to display.
        /// </summary>
        [Parameter]
        public string PlaceHolderText { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the value that is selected.
        /// </summary>
        [Parameter]
        public string SelectedValue { get; set; }

        /// <summary>
        /// Gets or sets the callback invoked when the selected value changes.
        /// </summary>
        [Parameter]
        public EventCallback<string> SelectedValueChanged { get; set; }

        /// <summary>
        /// Gets or sets the function to call to perform the search.
        /// </summary>
        [Parameter]
        public Func<string, Task<List<string>>> SearchFn { get; set; }

        /// <summary>
        /// Gets or sets a reference to the child input element.
        /// </summary>
        protected ElementReference InputElem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not an item has been selected.
        /// </summary>
        protected bool Selected { get; set; } = false;

        /// <summary>
        /// Gets the base index of the component.
        /// </summary>
        protected int BaseIndex { get; private set; }

        /// <summary>
        /// Gets or sets the list of possible values.
        /// </summary>
        protected List<string> Values { get; set; } = new ();

        /// <summary>
        /// Focus the control.
        /// </summary>
        /// <returns>The asynchronous task.</returns>
        public async Task FocusAsync() => await InputElem.FocusAsync();

        /// <summary>
        /// Gets the CSS class to display for the item based on whether it is selected.
        /// </summary>
        /// <param name="item">The item to get the CSS class for.</param>
        /// <returns>The class name.</returns>
        protected string GetClass(string item) =>
            index >= 0 && item == Values[index]
            ? "active"
            : string.Empty;

        /// <summary>
        /// Gets the tab index of the item.
        /// </summary>
        /// <param name="item">The item to compute the index for.</param>
        /// <returns>The index as a string.</returns>
        protected string GetIndex(string item) => Values.IndexOf(item) >= 0
? (Values.IndexOf(item) + BaseIndex).ToString()
: string.Empty;

        /// <summary>
        /// Called when parents have passed parameters down to child.
        /// </summary>
        /// <returns>Asynchronous task.</returns>
        protected override async Task OnParametersSetAsync()
        {
            if (!string.IsNullOrWhiteSpace(SelectedValue))
            {
                await SetSelectionAsync(SelectedValue);
            }

            await base.OnParametersSetAsync();
        }

        /// <summary>
        /// Handle keyboard navigation.
        /// </summary>
        /// <param name="e">The <see cref="KeyboardEventArgs"/>.</param>
        protected void HandleKeyDown(KeyboardEventArgs e)
        {
            var maxIndex = Values != null ?
                Values.Count - 1 : -1;

            switch (e.Key)
            {
                case KeyNames.ArrowDown:
                    if (index < maxIndex)
                    {
                        index++;
                    }

                    break;

                case KeyNames.ArrowUp:
                    if (index > 0)
                    {
                        index--;
                    }

                    break;

                case KeyNames.Enter:
                    if (Selected)
                    {
                        InvokeAsync(
                            async () =>
                            await SetSelectionAsync(string.Empty, true));
                    }
                    else if (index >= 0)
                    {
                        InvokeAsync(async () =>
                        await SetSelectionAsync(Values[index]));
                    }

                    break;
            }
        }

        /// <summary>
        /// Called when the value changes.
        /// </summary>
        /// <returns>The asynchronous task.</returns>
        protected async Task OnValueChangedAsync()
        {
            if (loading)
            {
                queued = true;
                return;
            }

            loading = true;

            do
            {
                queued = false;
                Values = await SearchFn(val);
            }
            while (queued);
            loading = false;
            await SetSelectionAsync(string.Empty);
            index = -1;
            StateHasChanged();
        }

        /// <summary>
        /// Called to set the selection.
        /// </summary>
        /// <param name="selection">The selection text.</param>
        /// <param name="reset">A value indicating whether the selection should be reset.</param>
        /// <returns>The asynchronous task.</returns>
        protected async Task SetSelectionAsync(string selection, bool reset = false)
        {
            if (string.IsNullOrWhiteSpace(selection))
            {
                if (Selected)
                {
                    Selected = false;
                    SelectedValue = string.Empty;
                    await SelectedValueChanged.InvokeAsync(string.Empty);
                    return;
                }
            }
            else
            {
                Selected = true;
                Values = null;
                if (SelectedValue != selection)
                {
                    SelectedValue = selection;
                    await SelectedValueChanged.InvokeAsync(selection);
                }
            }

            if (reset)
            {
                Value = string.Empty;
                await InputElem.FocusAsync();
            }
        }
    }
}
