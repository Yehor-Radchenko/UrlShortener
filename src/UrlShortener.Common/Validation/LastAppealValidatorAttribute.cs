using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Common.Validation;
public class LastAppealValidatorAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        DateTime lastAppeal = (DateTime)value;

        if (lastAppeal > DateTime.Now)
        {
            return new ValidationResult("LastAppeal cannot be in the future");
        }

        if (lastAppeal >= Convert.ToDateTime("2024-01-01") || lastAppeal == DateTime.MinValue)
        {
            return ValidationResult.Success;
        }

        return new ValidationResult("LastAppeal must be after 2024-01-01 or DateTime.MinValue");
    }
}
