using System;
using System.Threading;
using System.Threading.Tasks;

namespace PlanetaryDocs.Services
{
    /// <summary>
    /// App-wide loading service to keep track of asynchronous requests.
    /// </summary>
    public class LoadingService
    {
        private readonly Action noop = () => { };

        private int asyncCount;

        private Action stateChangedCallback = null;

        /// <summary>
        /// Gets or sets the callback to initiate a state change notification.
        /// </summary>
        public Action StateChangedCallback
        {
            get => stateChangedCallback ?? noop;
            set
            {
                if (stateChangedCallback != null)
                {
                    throw new InvalidOperationException("Only one callback can be registered at the root level.");
                }

                stateChangedCallback = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a loading operation is happening.
        /// </summary>
        public bool Loading => asyncCount > 0;

        /// <summary>
        /// Gets a value indicating whether the loading state changed based on
        /// the most recent transition.
        /// </summary>
        public bool StateChanged { get; private set; } = false;

        /// <summary>
        /// Start of an asynchronous operation.
        /// </summary>
        public void AsyncBegin()
        {
            StateChanged = false;
            Interlocked.Increment(ref asyncCount);
            if (asyncCount == 1)
            {
                StateChanged = true;
                StateChangedCallback();
            }
        }

        /// <summary>
        /// End of an asynchronous operation.
        /// </summary>
        public void AsyncEnd()
        {
            StateChanged = false;
            Interlocked.Decrement(ref asyncCount);

            if (asyncCount == 0)
            {
                StateChanged = true;
                StateChangedCallback();
            }
        }

        /// <summary>
        /// Single API to increment loading status, run the asynchronous operation,
        /// then decrement loading status.
        /// </summary>
        /// <param name="execution">The code to execute.</param>
        /// <param name="stateChanged">The code to execute if the loading status changes.</param>
        /// <remarks>The linear way to use the service looks like this:
        /// <code><![CDATA[
        ///     LoadingService.AsyncBegin();
        ///     var result = await ThingToDoAsync();
        ///     LoadingService.AsyncEnd();
        /// ]]></code>
        /// The non-linear (async op call) looks like:
        /// <code><![CDATA[
        ///     ResultType result;
        ///     await LoadingService.WrapExecutionAsync(
        ///         async () => {
        ///            result = await ThingsToDoAsync();
        ///         }
        ///     });
        /// ]]></code>
        /// </remarks>
        /// <returns>An asychronous task.</returns>
        public async Task WrapExecutionAsync(
            Func<Task> execution,
            Func<Task> stateChanged = null)
        {
            AsyncBegin();
            if (StateChanged && stateChanged != null)
            {
                await stateChanged();
            }

            try
            {
                await execution();
            }
            catch
            {
                throw;
            }
            finally
            {
                AsyncEnd();
                if (StateChanged && stateChanged != null)
                {
                    await stateChanged();
                }
            }
        }
    }
}
