using System;

namespace PlanetaryDocsLoader
{
    /// <summary>
    /// Progress bar for console.
    /// </summary>
    public class ProgressBar
    {
        private int current = 0;
        private int total;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressBar"/> class.
        /// </summary>
        /// <param name="total">Total items in list.</param>
        public ProgressBar(int total) => this.total = total;

        /// <summary>
        /// Advances the progress bar.
        /// </summary>
        /// <returns>The new bar.</returns>
        public string Advance()
        {
            current++;
            return ToString();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var pct = current * 100 / total;
            var progressBar = Console.BufferWidth - 6;
            var done = pct * progressBar / 100;
            var todo = progressBar - done;
            string doneStr = done == 0 ? string.Empty : new string('#', done);
            string toDoStr = todo == 0 ? string.Empty : new string('-', todo);
            return $"{doneStr}{toDoStr} {pct}%";
        }
    }
}
