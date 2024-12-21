using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using System.ComponentModel.DataAnnotations;

namespace WebBanGear.Models
{
	public class OrderModel
	{
		public int Id { get; set; }
		public string OrderCode { get; set; }
        public string CouponCode { get; set; }
        public decimal ShippingCost { get; set; }
        public string UserName { get; set; }
		public DateTime CreateDate { get; set; }
        public string Tinh { get; set; }
        public string Quan { get; set; }
        public string Phuong { get; set; }
        public int Status { get; set; }
		public AppUserModel User { get; set; }
		public List<OrderDetails> OrderDetails { get; set; }
	}
}
