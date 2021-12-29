using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace PlanetaryDocs.Shared
{
    /// <summary>
    /// Code for navigation component.
    /// </summary>
    public class NavMenuBase : ComponentBase
    {
        /// <summary>
        /// Gets the list of navigation items.
        /// </summary>
        protected NavItem[] NavItems
        { get; } = new[]
            {
                new NavItem
                {
                    Disabled = false,
                    Text = "Home",
                    Icon = "home",
                    Href = string.Empty,
                    Match = NavLinkMatch.All,
                },
                new NavItem { Disabled = true, Text = "View", Icon = "eye", Href = "/View" },
                new NavItem { Disabled = false, Text = "Add New", Icon = "plus", Href = "/Add" },
                new NavItem { Disabled = true, Text = "Edit", Icon = "pencil", Href = "/Edit" },
            };

        /// <summary>
        /// Gets or sets a value indicating whether the menu should be collapsed.
        /// </summary>
        protected bool CollapseNavMenu { get; set; } = true;

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        protected string Version { get; set; } = "?";

        /// <summary>
        /// Gets the CSS class to apply to the menu based on whether or not it is
        /// collapsed.
        /// </summary>
        protected string NavMenuCssClass => CollapseNavMenu ? "collapse" : null;

        /// <summary>
        /// Toggles the display of the menu.
        /// </summary>
        protected void ToggleNavMenu() => CollapseNavMenu = !CollapseNavMenu;

        /// <summary>
        /// Code to set version on initialization.
        /// </summary>
        protected override void OnInitialized()
        {
            if (Version == "?")
            {
                Version = System.Diagnostics.FileVersionInfo.GetVersionInfo(
                    GetType().Assembly.Location).ProductVersion;
            }

            base.OnInitialized();
        }

        /// <summary>
        /// Navigation item.
        /// </summary>
        protected class NavItem
        {
            /// <summary>
            /// Gets or sets a value indicating whether using this item as navigation
            /// is disabled.
            /// </summary>
            public bool Disabled { get; set; }

            /// <summary>
            /// Gets or sets the text to display for the navigation item.
            /// </summary>
            public string Text { get; set; }

            /// <summary>
            /// Gets or sets the icon to show.
            /// </summary>
            public string Icon { get; set; }

            /// <summary>
            /// Gets or sets the path to the target.
            /// </summary>
            public string Href { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the highlight logic should
            /// match based on the prefix of the current URL or the full URL.
            /// </summary>
            public NavLinkMatch Match { get; set; } = NavLinkMatch.Prefix;
        }
    }
}
