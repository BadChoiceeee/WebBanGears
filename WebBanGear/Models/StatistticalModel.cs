using System.ComponentModel.DataAnnotations;

namespace WebBanGear.Models
{
    public class StatistticalModel
    {
        [Key]
        public int Id { get; set; }
        public int Quantity { get; set; }// số lượng bán ra

        public int Sold { get; set; } //số lượng bán hàng
        public decimal Revenue { get; set; }//doanh thu
        public decimal Profit { get; set; }// lợi nhuâjn
        public DateTime DateCreated { get; set; }// ngày đặt hàng
    }
}
