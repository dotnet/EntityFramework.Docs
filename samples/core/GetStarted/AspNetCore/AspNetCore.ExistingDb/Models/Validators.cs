using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EFGetStarted.AspNetCore.ExistingDb.Models
{
	public class HashLengthAttribute : ValidationAttribute, IClientModelValidator
	{
		public HashLengthAttribute(string errorMessage = null) : base(errorMessage)
		{
			if (string.IsNullOrEmpty(errorMessage))
				ErrorMessage = $"Hash lenght must be 32 for {nameof(KindEnum.MD5)} and 64 for {nameof(KindEnum.SHA256)}";
			else
				ErrorMessage = errorMessage;
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var hi = (Hashes)validationContext.ObjectInstance;

			if (string.IsNullOrEmpty(hi.Search) ||
				hi.Kind == KindEnum.MD5 && hi.Search.Length != 32 ||
				hi.Kind == KindEnum.SHA256 && hi.Search.Length != 64)
			{
				return new ValidationResult(GetErrorMessage());
			}

			return ValidationResult.Success;
		}

		public void AddValidation(ClientModelValidationContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			MergeAttribute(context.Attributes, "data-val", "true");
			MergeAttribute(context.Attributes, "data-val-hashlength", GetErrorMessage());
		}

		private bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
		{
			if (attributes.ContainsKey(key))
			{
				return false;
			}

			attributes.Add(key, value);
			return true;
		}

		private string GetErrorMessage()
		{
			return ErrorMessageString;
		}
	}
}
