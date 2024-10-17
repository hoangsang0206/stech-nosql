using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STech.Data.Validations
{
    public class DateOfBirth : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            DateOnly maxDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-10));
            DateOnly minDate = DateOnly.Parse("1900-01-01");

            if(value is DateOnly date)
            {
                if(date < minDate || date > maxDate)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
}
