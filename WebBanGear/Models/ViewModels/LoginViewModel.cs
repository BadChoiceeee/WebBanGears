using System.ComponentModel.DataAnnotations;

namespace WebBanGear.Models.ViewModels
{
	public class LoginViewModel
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Bạn phải nhập UserName !")]
		public string Username { get; set; }

		[DataType(DataType.Password), Required(ErrorMessage = "Bạn phải nhập PassWord !")] // mã hoá mật khẩu
		public string Password { get; set; }
		public string ReturnUrl { get; set; }
	}
}
