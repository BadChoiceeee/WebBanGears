using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebBanGear.Repository.Validation;

namespace WebBanGear.Models
{
	public class ContactModel
	{
		[Key]
		[Required(ErrorMessage = "Yêu cầu nhập tiêu đề")]
		public string Name { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập Bản đồ ")]
		public string Map { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập Email ")]
		public string Email { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập SĐT ")]
		public string Phone { get; set; }

		[Required(ErrorMessage = "Yêu cầu nhập thông tin liên hệ")]
		public string Description { get; set; }
		public string LogoImg { get; set; }

		[NotMapped]
		[FileExtension]
		public IFormFile ImageUpload { get; set; }
	}
}
