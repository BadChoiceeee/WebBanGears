using System.ComponentModel.DataAnnotations;

namespace WebBanGear.Models.ViewModels
{
	public class ProductDetailsViewModel
	{
		public ProductModel ProductDetails { get; set; }


		[Required(ErrorMessage = "Yêu cầu nhập bình luận Sản phẩm")]
		public string Comment { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập tên")]
		public string Name { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập Email")]
		public string Email { get; set; }
	}
}
