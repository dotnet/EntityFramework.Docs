using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using PlanetaryDocs.Shared;

namespace PlanetaryDocs.Services
{
    /// <summary>
    /// Service to handle multi line edit controls.
    /// </summary>
    /// <remarks>
    /// Uses JavaScript interop to bypass SignalR restrictions on large fields.
    /// The component registers and an id is tracked on the client. As the user
    /// edits, the JavaScript calls back to the exposed static method to marshall
    /// the data back.
    /// </remarks>
    public class MultiLineEditService
    {
        /// <summary>
        /// Keeps track of services across all users.
        /// </summary>
        private static readonly Dictionary<string, MultiLineEditService> Services
            = new ();

        /// <summary>
        /// Components for a specific user.
        /// </summary>
        private readonly Dictionary<string, MultiLineEditBase> components
            = new ();

        private readonly IJSRuntime jsRuntime = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiLineEditService"/> class.
        /// </summary>
        /// <param name="jsRuntime">The JavaScript runtime.</param>
        public MultiLineEditService(IJSRuntime jsRuntime) =>
            this.jsRuntime = jsRuntime;

        /// <summary>
        /// Update the text.
        /// </summary>
        /// <param name="id">The unique id.</param>
        /// <param name="text">The text to update.</param>
        /// <returns>The asynchronous task.</returns>
        [JSInvokable]
        public static async Task UpdateTextAsync(string id, string text)
        {
            var service = Services[id];
            var component = service.components[id];
            await component.OnUpdateTextAsync(text);
        }

        /// <summary>
        /// Register the text to start the process.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="component">The <see cref="MultiLineEdit"/> instance.</param>
        /// <returns>A unique identifier.</returns>
        public async Task<string> RegisterTextAsync(
            string text,
            MultiLineEditBase component)
        {
            var id = Guid.NewGuid().ToString();
            await jsRuntime.InvokeVoidAsync(
                "markdownExtensions.setText",
                id,
                text,
                component.TextArea);
            components.Add(id, component);
            Services.Add(id, this);
            return id;
        }

        /// <summary>
        /// Unregister references.
        /// </summary>
        /// <param name="id">The id of the instance.</param>
        public void Unregister(string id)
        {
            var service = Services[id];
            Services.Remove(id);
            service.components.Remove(id);
        }
    }
}
