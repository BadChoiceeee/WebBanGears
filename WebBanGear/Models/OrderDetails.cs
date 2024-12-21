namespace WebBanGear.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string OrderCode { get; set; }
        public string CouponCode { get; set; }
        public long ProductId { get; set; }
        public decimal Price { get; set; }

        public string Tinh { get; set; }
        public string Quan { get; set; }
        public string Phuong { get; set; }
        public int Quantity { get; set; }
        public string FullName { get; set; } // Gộp tên thành một trường duy nhất
        public string Notes { get; set; }
        public string PhoneNumber { get; set; }
        public int OrderId { get; set; }
        public OrderModel Order { get; set; }
        public ProductModel Product { get; set; }
    }
}
