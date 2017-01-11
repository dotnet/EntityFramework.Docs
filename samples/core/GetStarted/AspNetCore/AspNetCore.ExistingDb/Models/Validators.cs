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
		//private int _year;

		public HashLengthAttribute(/*int Year*/)
		{
			//_year = Year;
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			HashesInfo hi = (HashesInfo)validationContext.ObjectInstance;

			if (hi.Kind == KindEnum.MD5 && hi.Search.Length != 32 ||
				hi.Kind == KindEnum.SHA256 && hi.Search.Length != 64)
			{
				return new ValidationResult(GetErrorMessage());
			}

			return ValidationResult.Success;
		}

		public void AddValidation(ClientModelValidationContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			MergeAttribute(context.Attributes, "data-val", "true");
			MergeAttribute(context.Attributes, "data-val-hashlength", GetErrorMessage());

			//var year = _year.ToString(CultureInfo.InvariantCulture);
			//MergeAttribute(context.Attributes, "data-val-classicmovie-year", year);
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
			return "Hash lenght must be 32 for MD5 and 64 for SHA256";
		}
	}
}
