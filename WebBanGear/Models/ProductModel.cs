using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebBanGear.Repository.Validation;

namespace WebBanGear.Models
{
    public class ProductModel
    {
        [Key]
        public long Id { get; set; }

		[Required(ErrorMessage = "Yêu cầu nhập tên Sản phẩm")]
		public string Name { get; set; }

        public string Slug { get; set; }

		[Required, MinLength(4, ErrorMessage = "Yêu cầu nhập Mô tả Sản phẩm")]
		public string Description { get; set; }

        [Range(0.01, 100000000, ErrorMessage = "Giá sản phẩm phải từ 0.01 đến 100,000,000")]
        [Required(ErrorMessage = "Yêu cầu nhập giá Sản phẩm")]
        [Column(TypeName = "decimal(20,2)")]
        public decimal Price { get; set; }

        [Range(0.01, 100000000, ErrorMessage = "Giá vốn sản phẩm phải từ 0.01 đến 100,000,000")]
        [Required(ErrorMessage = "Yêu cầu nhập giá vốn Sản phẩm")]
        [Column(TypeName = "decimal(20,2)")]
        public decimal CapitalPrice { get; set; }// giá vốn

        [Required, Range(1,int.MaxValue, ErrorMessage = "Chọn một thương hiệu")]

		public int BrandID { get; set; }

		[Required, Range(1, int.MaxValue, ErrorMessage = "Chọn một danh mục")]
		public int  CategoryId { get; set;}
        public CategoryModel Category { get; set; }
        public BrandModel Brand { get; set; }
		public string Image { get; set; }
		public int Quantity { get; set; }
		public int Sold { get; set; }
		public RatingModel Ratings { get; set; }
		[NotMapped]
        [FileExtension]
        public IFormFile ImageUpload { get; set; }
	}
}
