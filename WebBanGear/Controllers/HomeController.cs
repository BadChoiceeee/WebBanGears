using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebBanGear.Models;
using WebBanGear.Repository;

namespace WebBanGear.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUserModel> _usermanager;

        public HomeController(ILogger<HomeController> logger,DataContext context, UserManager<AppUserModel> userManager)
        {
            _logger = logger;
            _dataContext = context;
            _usermanager = userManager;
        }

        public async Task<IActionResult> Index(string startprice = "", string endprice = "")
        {
            var productsQuery = _dataContext.Products.Include("Category").Include("Brand").AsQueryable();

            // Lọc sản phẩm theo giá
            if (!string.IsNullOrEmpty(startprice) && !string.IsNullOrEmpty(endprice))
            {
                decimal startPriceValue;
                decimal endPriceValue;
                if (decimal.TryParse(startprice, out startPriceValue) && decimal.TryParse(endprice, out endPriceValue))
                {
                    productsQuery = productsQuery.Where(p => p.Price >= startPriceValue && p.Price <= endPriceValue);
                }
            }

            var products = await productsQuery.ToListAsync();

            var sliders = _dataContext.Sliders.Where(s => s.Status == 1).ToList();
            ViewBag.Sliders = sliders;

            return View(products);
        }


        public async Task<IActionResult> Contact()
		{
            var contact = _dataContext.Contact.FirstOrDefault(); 
            if (contact == null)
            {
                TempData["error"] = "Không tìm thấy thông tin liên hệ nào!";
                return RedirectToAction("Index", "Home");
            }
            return View(contact); 
        }

		public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statuscode)
        {
            if (statuscode == 404)
            {
                return View("NotFound");
            }
            else
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddWishList(long Id, WishlistModel wishlistModel)
        { 
            var user = await _usermanager.GetUserAsync(User);

            var wishlistProduct = new WishlistModel
            {
                ProductId = Id,
                UserId = user.Id,
            };

            _dataContext.Wishlists.Add(wishlistProduct);

            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, message = "Add to Wishllist successfully" });
            }
            catch (Exception )
            {
                return StatusCode(500, "An error occured while adding to Wishlist Table");
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddCompare(long Id)
        {
            var user = await _usermanager.GetUserAsync(User);

            var compareProduct = new CompareModel
            {
                ProductId = Id,
                UserId = user.Id,
            };

            _dataContext.Compares.Add(compareProduct);

            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, message = "Add to Compare successfully" });
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occured while adding to Compare Table");
            }
        }
		public async Task<IActionResult> Compare()
		{
			// Truy vấn thông tin về các sản phẩm và người dùng đã so sánh
			var compareData = await (from c in _dataContext.Compares
									 join p in _dataContext.Products on c.ProductId equals p.Id
									 join u in _dataContext.Users on c.UserId equals u.Id
									 select new CompareModel
									 {
										 Product = p,
										 UserId = u.Id,  // Bạn có thể muốn hiển thị thêm thông tin của người dùng
										 ProductId = p.Id // Lưu thông tin sản phẩm
									 }).ToListAsync();

			return View(compareData); // Trả về danh sách các sản phẩm so sánh với người dùng
		}

        public async Task<IActionResult> Wishlist()
        {
            // Lấy user hiện tại
            var user = await _usermanager.GetUserAsync(User);

            // Truy vấn thông tin về các sản phẩm trong wishlist của người dùng hiện tại
            var wishlistData = await (from w in _dataContext.Wishlists
                                      join p in _dataContext.Products on w.ProductId equals p.Id
                                      where w.UserId == user.Id // Lọc theo UserId của người dùng hiện tại
                                      select new WishlistModel
                                      {
                                          Product = p,
                                          UserId = w.UserId,  // Lưu UserId
                                          ProductId = p.Id    // Lưu ProductId
                                      }).ToListAsync();

            return View(wishlistData); // Trả về danh sách các sản phẩm trong wishlist của người dùng
        }

        public async Task<IActionResult> DeleteCompare(int Id)
        {
            try
            {
                var compare = await _dataContext.Compares.FirstOrDefaultAsync(c => c.ProductId == Id);

                if (compare == null)
                {
                    TempData["error"] = "Không tìm thấy sản phẩm trong danh sách so sánh!";
                    return RedirectToAction("Compare", "Home");
                }

                _dataContext.Compares.Remove(compare);
                await _dataContext.SaveChangesAsync();

                TempData["success"] = "Đã xóa thành công sản phẩm khỏi danh sách so sánh!";
            }
            catch (Exception ex)
            {
                TempData["error"] = "Đã xảy ra lỗi khi xóa sản phẩm: " + ex.Message;
            }

            return RedirectToAction("Compare", "Home");
        }
        public async Task<IActionResult> DeleteWishlist(int Id)
        {
            try
            {
                var wishlist = await _dataContext.Wishlists.FirstOrDefaultAsync(c => c.ProductId == Id);

                if (wishlist == null)
                {
                    TempData["error"] = "Không tìm thấy sản phẩm trong danh sách Wishlist!";
                    return RedirectToAction("Wishlist", "Home");
                }

                _dataContext.Wishlists.Remove(wishlist);
                await _dataContext.SaveChangesAsync();

                TempData["success"] = "Đã xóa thành công sản phẩm khỏi danh sách Wishlist!";
            }
            catch (Exception ex)
            {
                TempData["error"] = "Đã xảy ra lỗi khi xóa sản phẩm: " + ex.Message;
            }

            return RedirectToAction("Wishlist", "Home");
        }



    }
}
