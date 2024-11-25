using System.ComponentModel.DataAnnotations;

namespace EfficiencyHub.Common
{
    public class CompletedDateValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Проверка дали CompletedDate е зададена
            if (value is not DateTime completedDate)
            {
                return ValidationResult.Success; // Пропускаме, ако CompletedDate е null
            }

            // Вземане на стойността на CreatedDate от обекта
            var createdDateProperty = validationContext.ObjectType.GetProperty("CreatedDate");
            if (createdDateProperty == null)
            {
                return new ValidationResult("CreatedDate property not found.");
            }

            var createdDateValue = createdDateProperty.GetValue(validationContext.ObjectInstance) as DateTime?;

            if (createdDateValue == null)
            {
                return ValidationResult.Success; // Ако CreatedDate не е зададено, пропускаме
            }

            // Сравнение на датите
            if (completedDate < createdDateValue)
            {
                return new ValidationResult("Completed date cannot be earlier than created date.");
            }

            return ValidationResult.Success;
        }
    }
}
