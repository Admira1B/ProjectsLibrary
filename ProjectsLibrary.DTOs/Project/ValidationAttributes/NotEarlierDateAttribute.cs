using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class NotEarlierDateAttribute : ValidationAttribute, IClientModelValidator
{
    private readonly string _startDatePropertyName;
    public NotEarlierDateAttribute(string startDatePropertyName)
    {
        _startDatePropertyName = startDatePropertyName;
        ErrorMessage = "End date cannot be earlier than start date.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var endDate = value as DateTime?;
        if (!endDate.HasValue)
            return ValidationResult.Success;

        var startDateProperty = validationContext.ObjectType.GetProperty(_startDatePropertyName);
        if (startDateProperty == null)
            throw new ArgumentException("Property with this name not found");

        var startDateValue = startDateProperty.GetValue(validationContext.ObjectInstance) as DateTime?;

        if (!startDateValue.HasValue)
            return ValidationResult.Success;

        if (endDate < startDateValue)
            return new ValidationResult(ErrorMessage);

        return ValidationResult.Success;
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        // Включаем клиентскую валидацию
        context.Attributes.Add("data-val", "true");
        // Параметр для нашего кастомного валидатора — имя свойства начала периода
        context.Attributes.Add("data-val-notearlierdate", ErrorMessage);
        context.Attributes.Add("data-val-notearlierdate-startdateproperty", _startDatePropertyName);
    }
}