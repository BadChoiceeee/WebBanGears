using System.ComponentModel.DataAnnotations;
namespace WebBanGear.Models.ViewModels
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        public string FullName { get; set; }

        public decimal ShippingCost { get; set; }
        public string CouponCode { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        public string PhoneNumber { get; set; }

        // Giữ nguyên các trường hiện có
        public List<CartItemModel> CartItems { get; set; }

        // Thêm trường Notes
        public string Notes { get; set; }

        // Thêm trường GrandTotal
        public decimal GrandTotal { get; set; }
		// Thêm các trường cho địa chỉ
		public string Tinh { get; set; }
		public string Quan { get; set; }
		public string Phuong { get; set; }

	}
}
