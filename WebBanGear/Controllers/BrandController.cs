using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanGear.Models;
using WebBanGear.Repository;

namespace WebBanGear.Controllers
{
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;

        public BrandController(DataContext context)
        {
            _dataContext = context;
        }

        public async Task<IActionResult> Index(string Slug = "", string sort_by = "", string startprice = "", string endprice = "")
        {
            // Lấy thông tin brand dựa trên Slug
            BrandModel brand = _dataContext.Brands.Where(b => b.Slug == Slug).FirstOrDefault();

            if (brand == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Slug = Slug;

            // Lấy danh sách sản phẩm theo brand
            IQueryable<ProductModel> productsByBrand = _dataContext.Products.Where(p => p.BrandID == brand.Id);
            var count = await productsByBrand.CountAsync();

            if (count > 0)
            {
                // Lọc và sắp xếp sản phẩm theo các điều kiện
                if (sort_by == "price_increase")
                {
                    productsByBrand = productsByBrand.OrderBy(p => p.Price);
                }
                else if (sort_by == "price_decrease")
                {
                    productsByBrand = productsByBrand.OrderByDescending(p => p.Price);
                }
                else if (sort_by == "price_newest")
                {
                    productsByBrand = productsByBrand.OrderByDescending(p => p.Id);
                }
                else if (sort_by == "price_oldest")
                {
                    productsByBrand = productsByBrand.OrderBy(p => p.Id);
                }
                else if (startprice != "" && endprice != "")
                {
                    decimal startPriceValue;
                    decimal endPriceValue;
                    if (decimal.TryParse(startprice, out startPriceValue) && decimal.TryParse(endprice, out endPriceValue))
                    {
                        productsByBrand = productsByBrand.Where(p => p.Price >= startPriceValue && p.Price <= endPriceValue);
                    }
                    else
                    {
                        productsByBrand = productsByBrand.OrderByDescending(p => p.Id);
                    }
                }
                else
                {
                    productsByBrand = productsByBrand.OrderByDescending(p => p.Id);
                }
            }

            return View(await productsByBrand.ToListAsync());
        }
    }
}
