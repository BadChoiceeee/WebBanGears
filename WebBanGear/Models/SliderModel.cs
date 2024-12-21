using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebBanGear.Repository.Validation;

namespace WebBanGear.Models
{
	public class SliderModel
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Yêu cầu nhập tên Slider")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Yêu cầu nhập Mô tả Slider")]
		public string Description { get; set; }
		public int? Status { get; set; }
		public string Image { get; set; }

		[NotMapped]
		[FileExtension]
		public IFormFile ImageUpload { get; set; }
	}
}
