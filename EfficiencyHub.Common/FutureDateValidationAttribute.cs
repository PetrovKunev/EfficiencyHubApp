using System.ComponentModel.DataAnnotations;

namespace EfficiencyHub.Common
{
    public class FutureDateValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not DateTime completedDate)
            {
                return ValidationResult.Success;
            }

            if (completedDate > DateTime.UtcNow)
            {
                return new ValidationResult("Completed date cannot be a future date.");
            }

            return ValidationResult.Success;
        }
    }
}
