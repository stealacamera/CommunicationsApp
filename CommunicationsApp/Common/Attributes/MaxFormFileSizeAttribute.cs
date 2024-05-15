using System.ComponentModel.DataAnnotations;

namespace CommunicationsApp.Web.Common.Attributes;

public sealed class MaxFormFileMBSizeAttribute : ValidationAttribute
{
    private readonly double _maxSize;
    private readonly string _errorMessage;

    public MaxFormFileMBSizeAttribute(double maxSize)
    {
        _maxSize = maxSize;
        _errorMessage = $"Maximum file size allowed is {_maxSize} MB";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;

        var valueType = value.GetType();
        double maxBytes = _maxSize * 1000000;

        if (typeof(IFormFile).IsAssignableFrom(valueType))
        {
            if (((IFormFile)value).Length > maxBytes)
                return new ValidationResult(_errorMessage);
        }
        else if (typeof(IFormFileCollection).IsAssignableFrom(valueType))
        {
            var files = (IFormFileCollection)value;

            foreach (var file in files)
            {
                if (file.Length > maxBytes)
                    return new ValidationResult(_errorMessage);
            }
        }
        else
            return new ValidationResult("Unaccepted value type");

        return ValidationResult.Success;
    }
}
