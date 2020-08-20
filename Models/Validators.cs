using System;
using System.ComponentModel.DataAnnotations;
namespace TheWall.Models
{
    public class PastAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if ((DateTime)value < DateTime.Today)
            {
                return new ValidationResult("No dates in the past are allowed");
            }
            return ValidationResult.Success;
        }
    }
}