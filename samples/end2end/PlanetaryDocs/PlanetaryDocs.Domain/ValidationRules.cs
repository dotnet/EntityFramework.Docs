using System;
using System.Linq;

namespace PlanetaryDocs.Domain
{
    /// <summary>
    /// Validation rules for the project.
    /// </summary>
    public static class ValidationRules
    {
        /// <summary>
        /// Allowed punctuation.
        /// </summary>
        private const string Punctuation = "(),?!'\".";

        /// <summary>
        /// Punctuation allowed in the unique document identifier.
        /// </summary>
        private const string UidAllowed = "_.-";

        /// <summary>
        /// The low range of alpha.
        /// </summary>
        private const string LowRange = "az";

        /// <summary>
        /// The high range of alpha.
        /// </summary>
        private const string HighRange = "AZ";

        /// <summary>
        /// Numbers range.
        /// </summary>
        private const string Numbers = "09";

        /// <summary>
        /// Generate a valid <see cref="ValidationState"/>.
        /// </summary>
        /// <returns>The <see cref="ValidationState"/>.</returns>
        public static ValidationState ValidResult() =>
            new () { IsValid = true };

        /// <summary>
        /// Generate an invalid <see cref="ValidationState"/>.
        /// </summary>
        /// <param name="message">The validation message.</param>
        /// <returns>The <see cref="ValidationState"/>.</returns>
        public static ValidationState InvalidResult(string message) => new ()
        {
            IsValid = false,
            Message = message,
        };

        /// <summary>
        /// Handles multiple results and returns the first invalid result.
        /// </summary>
        /// <param name="fieldName">The name of the field.</param>
        /// <param name="fieldValue">The value.</param>
        /// <param name="validations">The validations to apply.</param>
        /// <returns>Either a valid result or the first invalid result.</returns>
        public static ValidationState CompoundResult(
            string fieldName,
            string fieldValue,
            params Func<string, string, ValidationState>[] validations)
        {
            var result = ValidResult();
            foreach (var validation in validations)
            {
                result = validation(fieldName, fieldValue);
                if (!result.IsValid)
                {
                    return result;
                }
            }

            return result;
        }

        /// <summary>
        /// Validate that content exists.
        /// </summary>
        /// <param name="fieldName">The name of the field.</param>
        /// <param name="val">The value.</param>
        /// <returns>The <see cref="ValidationState"/>.</returns>
        public static ValidationState IsRequired(
            string fieldName,
            string val) => string.IsNullOrWhiteSpace(val) ?
   InvalidResult($"{fieldName} is required.")
   : ValidResult();

        /// <summary>
        /// Validates only alpha allowed.
        /// </summary>
        /// <param name="fieldName">The name of the field.</param>
        /// <param name="val">The value.</param>
        /// <returns>The <see cref="ValidationState"/>.</returns>
        public static ValidationState IsAlphaOnly(
            string fieldName,
            string val)
        {
            if (string.IsNullOrWhiteSpace(val))
            {
                return InvalidResult($"Field '{fieldName}' cannot be null or empty.");
            }

            return val.All(c => (c >= LowRange[0] && c <= LowRange[1])
            || (c >= HighRange[0] && c <= HighRange[1])) ?
                ValidResult()
                : InvalidResult($"Field '{fieldName}' contains non-alpha characters.");
        }

        /// <summary>
        /// Validates allowed alphanumeric and punctuation.
        /// </summary>
        /// <param name="fieldName">The name of the field.</param>
        /// <param name="val">The value.</param>
        /// <param name="uidCheck">A value indicating whether to restrict punctuation to valid a document id.</param>
        /// <returns>The <see cref="ValidationState"/>.</returns>
        public static ValidationState IsSimpleText(
            string fieldName,
            string val,
            bool uidCheck = false)
        {
            var valid = true;
            var limits = new[] { LowRange, HighRange, Numbers };

            foreach (var c in val.AsSpan())
            {
                if (!valid)
                {
                    break;
                }

                if (c == ' ')
                {
                    continue;
                }

                if (!uidCheck && Punctuation.Contains(c))
                {
                    continue;
                }

                if (uidCheck && UidAllowed.Contains(c))
                {
                    continue;
                }

                var rangeCheck = false;
                foreach (var range in limits)
                {
                    if (rangeCheck)
                    {
                        break;
                    }

                    if (c >= range[0] && c <= range[1])
                    {
                        rangeCheck = true;
                        continue;
                    }
                }

                valid = rangeCheck;
            }

            return valid ? ValidResult() :
                InvalidResult($"Field '{fieldName}' contains invalid characters.");
        }

        /// <summary>
        /// Master validation routes based on property name.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value.</param>
        /// <returns>The <see cref="ValidationState"/>.</returns>
        public static ValidationState ValidateProperty(
            string name,
            string value) => name switch
            {
                nameof(Document.AuthorAlias) => CompoundResult(
                                       name,
                                       value,
                                       IsRequired,
                                       IsAlphaOnly),

                nameof(Document.Description) => IsRequired(name, value),

                nameof(Document.Markdown) => IsRequired(name, value),

                nameof(Document.Title) => CompoundResult(
                    name,
                    value,
                    IsRequired,
                    (n, v) => IsSimpleText(n, v, false)),

                nameof(Document.Uid) => CompoundResult(
                    name,
                    value,
                    IsRequired,
                    (n, v) => IsSimpleText(n, v, true)),

                _ => InvalidResult("Unknown property."),
            };

        /// <summary>
        /// Validates all fields on the <see cref="Document"/>.
        /// </summary>
        /// <param name="doc">The <see cref="Document"/> to validate.</param>
        /// <returns>The <see cref="ValidationState"/>.</returns>
        public static ValidationState[] ValidateDocument(Document doc) =>
            doc == null
                ? new[] { InvalidResult("Document cannot be null") }
                : new[]
                {
                    ValidateProperty(nameof(Document.Uid), doc.Uid),
                    ValidateProperty(nameof(Document.AuthorAlias), doc.AuthorAlias),
                    ValidateProperty(nameof(Document.Description), doc.Description),
                    ValidateProperty(nameof(Document.Markdown), doc.Markdown),
                    ValidateProperty(nameof(Document.Title), doc.Title),
                };
    }
}
