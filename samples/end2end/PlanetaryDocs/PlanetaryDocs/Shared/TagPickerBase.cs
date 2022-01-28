using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace PlanetaryDocs.Shared
{
    /// <summary>
    /// Code for the <see cref="TagPicker"/> component.
    /// </summary>
    public class TagPickerBase : ComponentBase
    {
        private string newTag;

        /// <summary>
        /// Gets or sets the list of tags to choose from.
        /// </summary>
        [Parameter]
        public List<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the callback to notify when tags change.
        /// </summary>
        [Parameter]
        public EventCallback<List<string>> TagsChanged { get; set; }

        /// <summary>
        /// Gets or sets the tab index.
        /// </summary>
        [Parameter]
        public string TabIndex { get; set; }

        /// <summary>
        /// Gets or sets the new tag.
        /// </summary>
        public string NewTag
        {
            get => newTag;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) &&
                    !Tags.Contains(value))
                {
                    Tags.Add(value);
                    newTag = string.Empty;
                    AddTag = string.Empty;
                    PickNew = false;
                    InvokeAsync(async () => await TagsChanged.InvokeAsync(
                        Tags.ToList()));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user is picking
        /// a new tag.
        /// </summary>
        protected bool PickNew { get; set; } = false;

        /// <summary>
        /// Gets or sets the name of a new tag to add.
        /// </summary>
        protected string AddTag { get; set; } = string.Empty;

        /// <summary>
        /// Gets the base index for tabs.
        /// </summary>
        protected int BaseIndex =>
            int.TryParse(TabIndex, out var idx) ? idx :
            100;

        /// <summary>
        /// Gets the tab index after the tag list.
        /// </summary>
        protected int AltTabIndex =>
            BaseIndex + Tags.Count;

        /// <summary>
        /// Handles removing a tag from the list.
        /// </summary>
        /// <param name="tag">The tag to remove.</param>
        /// <returns>An asynchronous task.</returns>
        public async Task RemoveAsync(string tag)
        {
            Tags.Remove(tag);
            await TagsChanged.InvokeAsync(Tags.ToList());
        }

        /// <summary>
        /// Gets the tab index for a tag.
        /// </summary>
        /// <param name="tag">The tag to index.</param>
        /// <returns>The tab index.</returns>
        protected int IndexForTag(string tag) =>
            BaseIndex + Tags.IndexOf(tag) + 1;
    }
}
