using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace PlanetaryDocs.Services
{
    /// <summary>
    /// Service to handle back button.
    /// </summary>
    public class HistoryService
    {
        private readonly Func<ValueTask> goBack;

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryService"/> class.
        /// </summary>
        /// <param name="jsRuntime">The implementation of the JavaScript runtime.</param>
        public HistoryService(IJSRuntime jsRuntime) =>
            goBack = () => jsRuntime.InvokeVoidAsync("history.go", "-1");

        /// <summary>
        /// Exposes the go back function.
        /// </summary>
        /// <returns>An asynchronous task.</returns>
        public ValueTask GoBackAsync() => goBack();
    }
}
