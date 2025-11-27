using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ProjectsLibrary.DTOs.Project.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    internal class NotPastDateAttribute : ValidationAttribute, IClientModelValidator
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime dateValue)
            {
                return dateValue.Date >= DateTime.Today;
            }
            return false;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-notpastdate", ErrorMessage ?? "End date cannot be earlier than today.");
        }
    }
}