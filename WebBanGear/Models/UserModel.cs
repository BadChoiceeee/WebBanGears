using System.ComponentModel.DataAnnotations;

namespace WebBanGear.Models
{
	public class UserModel
	{
		public int Id { get; set; }

		[Required(ErrorMessage ="Bạn phải nhập UserName !")]
		public string Username { get; set; }

		[Required(ErrorMessage = "Bạn phải nhập Email !"),	EmailAddress] // dạng email 
		public string Email { get; set; }
		[DataType(DataType.Password),Required(ErrorMessage ="Bạn phải nhập PassWord !")] // mã hoá mật khẩu
		public string Password { get; set; }
	}
}
