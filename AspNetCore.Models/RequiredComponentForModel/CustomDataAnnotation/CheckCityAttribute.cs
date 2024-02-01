using AspNetCore.Models.Enumerators;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Models.RequiredComponentForModel.CustomDataAnnotation
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
		AllowMultiple = false)]
	public class CheckCityAttribute : ValidationAttribute, IClientModelValidator
	{
		public override bool IsValid(object? value)
		{
			try
			{
				if (value != null) 
				{
					IEnumerable<string> AvailableCity = StaticDefinitions.StaticLists.Cities;
					if (AvailableCity.Contains(value.ToString()))
						return true;
				}
			}catch (Exception ex)
			{
				return false;
			}
			return false;
		}
		public void AddValidation(ClientModelValidationContext context)
		{
			context.Attributes.Add("data-val", "true");
			context.Attributes.Add("data-val-checkcity",
									"Select Valid City");
			string listOfCountries = string.Join(',', StaticDefinitions.StaticLists.Cities);
			context.Attributes.Add("data-list-of-cities", listOfCountries);
		}
	}
}
