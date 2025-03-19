namespace EcommerceRestApi.Helpers
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class DateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateGreaterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = (DateTime)value;
            var comparisonProperty = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (comparisonProperty == null)
                return new ValidationResult($"Unknown property: {_comparisonProperty}");

            var comparisonValue = (DateTime)comparisonProperty.GetValue(validationContext.ObjectInstance);

            if (currentValue <= comparisonValue)
                return new ValidationResult(ErrorMessage ?? "Estimated arrival date must be after shipment date.");

            return ValidationResult.Success;
        }
    }

}
