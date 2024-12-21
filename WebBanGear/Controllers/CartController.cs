using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanGear.Models;
using WebBanGear.Models.ViewModels;
using WebBanGear.Repository;


namespace WebBanGear.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext _dataContext;
        public CartController(DataContext _context)
        {
            _dataContext = _context;
        }
        public IActionResult Index()
        {
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemViewModel cartVM = new()
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(x => x.Quantity * x.Price)
            };
            return View(cartVM);
        }
        public ActionResult Checkout()
        {
            return View("~/Views/Checkout/Index.cshtml");
        }
        [HttpPost]
        public async Task<IActionResult> Add(long Id)
        {
            // Lấy sản phẩm từ cơ sở dữ liệu
            ProductModel product = await _dataContext.Products.FindAsync(Id);

            // Kiểm tra nếu sản phẩm không tồn tại
            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });
            }

            // Lấy giỏ hàng từ session hoặc tạo mới nếu chưa có
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            // Tìm sản phẩm trong giỏ hàng
            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();

            // Kiểm tra nếu sản phẩm đã có trong giỏ hàng
            if (cartItem == null)
            {
                // Nếu chưa, thêm sản phẩm vào giỏ hàng
                if (product.Quantity > 0)
                {
                    cart.Add(new CartItemModel(product));
                }
                else
                {
                    return Json(new { success = false, message = "Sản phẩm đã hết hàng." });
                }
            }
            else
            {
                // Nếu có, kiểm tra số lượng
                if (cartItem.Quantity < product.Quantity)
                {
                    cartItem.Quantity += 1;
                }
                else
                {
                    return Json(new { success = false, message = "Vượt quá số lượng sản phẩm có sẵn!" });
                }
            }

            // Cập nhật giỏ hàng vào session
            HttpContext.Session.SetJson("Cart", cart);

            // Trả về phản hồi thành công
            return Json(new { success = true, message = "Thêm sản phẩm vào giỏ hàng thành công!" });
        }


        // giảm số lượng trong cart
        public async Task<IActionResult> Decrease(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();
            if (cartItem.Quantity > 1)
            {
                --cartItem.Quantity;
            }
            else
            {
                cart.RemoveAll(p => p.ProductId == Id);
            }

            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
			TempData["success"] = "Giảm số lượng sản phẩm thành công !";
			return RedirectToAction("Index");
        }
        //tăng số lượng trong cart
        public async Task<IActionResult> Increase(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();

            ProductModel product = await _dataContext.Products.Where(p => p.Id == Id).FirstOrDefaultAsync();
            if (cartItem.Quantity >= 1 && product.Quantity > cartItem.Quantity)
            {
                ++cartItem.Quantity;
                TempData["success"] = "Tăng số lượng sản phẩm thành công !";
            }
            else
            {
                cartItem.Quantity = product.Quantity;
                TempData["error"] = $"Vượt quá số lượng sản phẩm có sẵn!";
            }

            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
			return RedirectToAction("Index");
        }
        public async Task<IActionResult> Remove(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            cart.RemoveAll(p => p.ProductId == Id);
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
			TempData["success"] = "Xoá sản phẩm thành công !";
			return RedirectToAction("Index");
        }
        public async Task<IActionResult> Clear()
        {
			HttpContext.Session.Remove("Cart");
			TempData["success"] = "Xoá toàn bộ Giỏ hàng thành công !";
			return RedirectToAction("Index");
		}
	}
}
