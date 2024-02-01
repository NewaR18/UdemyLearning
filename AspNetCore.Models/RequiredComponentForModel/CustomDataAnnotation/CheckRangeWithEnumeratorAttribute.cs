using AspNetCore.Models.Enumerators;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Models.CustomDataAnnotation
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
		AllowMultiple = false)]
	public class CheckRangeWithEnumeratorAttribute: ValidationAttribute, IClientModelValidator
	{
		//The above example is for learning custom validation in unobstructive way
		//Otherwise it can be easily achieved by using Required attribute
		public override bool IsValid(object? value)
		{
			// Abandoned as custom valiadation attribute jQuery validation is not working of radios
			/*
			if (value == null)
			{
				return false;
			}
			try
			{
				if (value.ToString()!.Length >= 5)
					return true;
				int maxValueOfEnum = Enum.GetValues(typeof(PaymentMethodEnum)).Cast<int>().Max();
				int minValueOfEnum = Enum.GetValues(typeof(PaymentMethodEnum)).Cast<int>().Min();
				if (Convert.ToInt32(value) <= maxValueOfEnum
					&&
					Convert.ToInt32(value) >= minValueOfEnum
					)
				{
					return true;
				}
			}catch (Exception ex)
			{
				return false;
			}*/
			return false;
		}
		public void AddValidation(ClientModelValidationContext context)
		{
			// Abandoned as custom valiadation attribute jQuery validation is not working of radios
			/*
			context.Attributes.Add("data-val", "true");
			context.Attributes.Add("data-val-checkenumrange",
									"Please Select At least one of them");
			context.Attributes.Add("data-min-value-of-enum", Enum.GetValues(typeof(PaymentMethodEnum)).Cast<int>().Min().ToString());
			context.Attributes.Add("data-max-value-of-enum", Enum.GetValues(typeof(PaymentMethodEnum)).Cast<int>().Max().ToString());*/
		}
	}
}
