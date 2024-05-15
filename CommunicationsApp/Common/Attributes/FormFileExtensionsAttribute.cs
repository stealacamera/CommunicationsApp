using System.ComponentModel.DataAnnotations;

namespace CommunicationsApp.Web.Common.Attributes;

public sealed class FormFileExtensionsAttribute : ValidationAttribute
{
    private readonly string[] _legalExtensions;
    private readonly string _errorMessage;

    public FormFileExtensionsAttribute(string legalExtensions)
    {
        _legalExtensions = legalExtensions.Split(", ").Select(e => e.ToLower()).ToArray();
        _errorMessage = $"Unaccepted file type: acceptable types are {string.Join(", ", _legalExtensions)}";
    }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;

        var valueType = value.GetType();

        if (typeof(IFormFile).IsAssignableFrom(valueType))
        {
            if (!IsFileExtensionValid((IFormFile)value))
                return new ValidationResult(_errorMessage);

            return ValidationResult.Success;
        }
        else if (typeof(IFormFileCollection).IsAssignableFrom(valueType))
        {
            var files = (IFormFileCollection)value;

            foreach (var file in files)
            {
                if (!IsFileExtensionValid(file))
                    return new ValidationResult(_errorMessage);
            }

            return ValidationResult.Success;
        }

        return new ValidationResult("Unaccepted value type");
    }

    private bool IsFileExtensionValid(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).ToLower();
        return _legalExtensions.Contains(extension);
    }
}