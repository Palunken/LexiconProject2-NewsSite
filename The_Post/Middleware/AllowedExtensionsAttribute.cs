using System.ComponentModel.DataAnnotations;

namespace The_Post.Middleware
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                // Get the file extension
                var extension = Path.GetExtension(file.FileName)?.ToLower();
                if (extension == null || !_extensions.Contains(extension.TrimStart('.')))
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }
            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"Please select an image file ({string.Join(", ", _extensions.Select(e => "." + e))}).";
        }
    }
}
