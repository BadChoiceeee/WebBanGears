using System.ComponentModel.DataAnnotations;

namespace WebBanGear.Models
{
	public class ProductQuantityModel
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Yêu cầu nhập số lượng")]
		public int Quantity { get; set; }
		
		public long ProductId { get; set; }
		public DateTime DateCreated { get; set; }
	}
}
