namespace WebBanGear.Models
{
    public class ShippingModel
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string Ward { get; set; }// phường,xã
        public string District { get; set; } //quận,huyện
        public string City { get; set; }// thành phố
    }
}
