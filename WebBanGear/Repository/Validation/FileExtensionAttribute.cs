using System.ComponentModel.DataAnnotations;

namespace WebBanGear.Repository.Validation
{
	public class FileExtensionAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value is  IFormFile file) 
			{
				var extension = Path.GetExtension(file.FileName);//abc.jpg
				string[] extensions = { "jpg", "png", "jpeg" };

				bool result = extensions.Any(x => extension.EndsWith(x));

				if (!result) 
				{
					return new ValidationResult("Cho phép sử dụng ảnh có định dạng jpg, png, jepg");

				}
			}
			return ValidationResult.Success;
		}
	}
}
