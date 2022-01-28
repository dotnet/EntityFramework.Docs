using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using PlanetaryDocs.Services;

namespace PlanetaryDocs.Shared
{
    /// <summary>
    /// Code for the <see cref="MultiLineEdit"/> component.
    /// </summary>
    /// <remarks>
    /// This component enables editing of extremely large fields by bypassing the
    /// SignalR communication and using JavaScript interop to marshall values.
    /// </remarks>
    public class MultiLineEditBase : ComponentBase, IDisposable
    {
        private string id;

        /// <summary>
        /// Gets or sets the <see cref="MultiLineEditService"/>.
        /// </summary>
        [Inject]
        public MultiLineEditService EditService { get; set; }

        /// <summary>
        /// Gets or sets the tab index.
        /// </summary>
        [Parameter]
        public string TabIndex { get; set; }

        /// <summary>
        /// Gets or sets the reference to the text area used for editing.
        /// </summary>
        public ElementReference TextArea { get; set; }

        /// <summary>
        /// Gets or sets the text to edit.
        /// </summary>
        [Parameter]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the callback to notify on text changed.
        /// </summary>
        [Parameter]
        public EventCallback<string> TextChanged { get; set; }

        /// <summary>
        /// Implement dispose to remove references.
        /// </summary>
        public void Dispose()
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                EditService.Unregister(id);
                id = string.Empty;
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Called to update the text from JavaScript.
        /// </summary>
        /// <param name="text">The text to update.</param>
        /// <returns>The asynchronous task.</returns>
        public async Task OnUpdateTextAsync(string text)
        {
            Text = text;
            await TextChanged.InvokeAsync(text);
        }

        /// <summary>
        /// After rendering, hook up the interop.
        /// </summary>
        /// <param name="firstRender">A value indicating whether it is the first render.</param>
        /// <returns>The asynchronous task.</returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                id = await EditService.RegisterTextAsync(
                    Text,
                    this);
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
