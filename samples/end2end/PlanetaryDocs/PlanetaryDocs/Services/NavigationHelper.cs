using System;
using System.Collections.Generic;
using System.Linq;
using Web = System.Net.WebUtility;

namespace PlanetaryDocs.Services
{
    /// <summary>
    /// Helper for creating navigation links.
    /// </summary>
    public static class NavigationHelper
    {
        /// <summary>
        /// Link to view a document.
        /// </summary>
        /// <param name="uid">The unique identifier.</param>
        /// <param name="auditId">The audit id.</param>
        /// <returns>The view link.</returns>
        public static string ViewDocument(
            string uid,
            Guid auditId = default) =>
                auditId == default
                ? $"/View/{Web.UrlEncode(uid)}"
                : $"/View/{Web.UrlEncode(uid)}?history={Web.UrlEncode(auditId.ToString())}";

        /// <summary>
        /// Link to edit a document.
        /// </summary>
        /// <param name="uid">The unique identifier.</param>
        /// <returns>The view link.</returns>
        public static string EditDocument(string uid) =>
            $"/Edit/{Web.UrlEncode(uid)}";

        /// <summary>
        /// Decomposes the query string.
        /// </summary>
        /// <param name="uri">The full uri.</param>
        /// <returns>The query string values.</returns>
        public static IDictionary<string, string> GetQueryString(string uri)
        {
            var pairs = new Dictionary<string, string>();

            var queryString = uri.Split('?');

            if (queryString.Length < 2)
            {
                return pairs;
            }

            var keyValuePairs = queryString[1].Split('&');

            foreach (var keyValuePair in keyValuePairs)
            {
                if (keyValuePair.IndexOf('=') > 0)
                {
                    var pair = keyValuePair.Split('=');
                    pairs.Add(pair[0], Web.UrlDecode(pair[1]));
                }
            }

            return pairs;
        }

        /// <summary>
        /// Create a query string from key value pairs.
        /// </summary>
        /// <param name="values">The values to use.</param>
        /// <returns>The composed query string.</returns>
        public static string CreateQueryString(
            params (string key, string value)[] values)
        {
            var queryString =
                string.Join(
                    '&',
                    values.Select(
                        v => $"{v.key}={Web.UrlEncode(v.value)}"));
            return queryString;
        }
    }
}
