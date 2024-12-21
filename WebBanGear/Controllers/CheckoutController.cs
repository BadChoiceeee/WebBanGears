using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using WebBanGear.Models;
using WebBanGear.Models.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using WebBanGear.Repository;
using WebBanGear.Areas.Admin.Repository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace WebBanGear.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IEmailSender _emailSender;

        public CheckoutController(IEmailSender emailSender, DataContext context)
        {
            _dataContext = context;
            _emailSender = emailSender;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Checkout()// view checkout
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            if (cartItems == null || cartItems.Count == 0)
            {
                TempData["error"] = "Giỏ hàng trống. Vui lòng thêm sản phẩm vào giỏ hàng trước khi thanh toán.";
                return RedirectToAction("Index", "Cart");
            }
            //Nhận giá vận chuyển từ cookie
            var shippingPriceCookie = Request.Cookies["ShippingPrice"];
            decimal shippingPrice = 0;
            //Nhận coupon code
            var coupon_code = Request.Cookies["CouponTitle"];

            if (shippingPriceCookie != null)
            {
                var shippingPriceJson = shippingPriceCookie;
                shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceJson);// giải dữ liệu kiểu json
            }

            decimal grandTotal = cartItems.Sum(item => item.Price * item.Quantity);

            var model = new CheckoutViewModel
            {
                CartItems = cartItems,
                GrandTotal = grandTotal,
                ShippingCost = shippingPrice,
                CouponCode = coupon_code
            };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Checkout(CheckoutViewModel model) // checkout đặt hàng
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            if (cartItems == null || cartItems.Count == 0)
            {
                TempData["error"] = "Giỏ hàng trống. Vui lòng thêm sản phẩm vào giỏ hàng trước khi thanh toán.";
                return RedirectToAction("Index", "Cart");
            }

            // Nhận giá vận chuyển từ cookie
            var shippingPriceCookie = Request.Cookies["ShippingPrice"];
            decimal shippingPrice = 0;
            var coupon_code = Request.Cookies["CouponTitle"];

            if (shippingPriceCookie != null)
            {
                var shippingPriceJson = shippingPriceCookie;
                shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceJson);
            }

            if (ModelState.IsValid)
            {
                var orderCode = Guid.NewGuid().ToString();
                var orderItem = new OrderModel
                {
                    OrderCode = orderCode,
                    UserName = userEmail,
                    Status = 1,
                    ShippingCost = shippingPrice,
                    CreateDate = DateTime.Now,
                    // Lưu thông tin địa chỉ vào OrderModel
                    Tinh = model.Tinh,
                    Quan = model.Quan,
                    Phuong = model.Phuong
                };

                _dataContext.Add(orderItem);
                await _dataContext.SaveChangesAsync(); // Lưu đơn hàng trước để tạo OrderCode

                foreach (var cart in cartItems)
                {
                    var orderDetails = new OrderDetails
                    {
                        UserName = userEmail,
                        OrderCode = orderCode,
                        ProductId = cart.ProductId,
                        CouponCode = coupon_code,
                        Price = cart.Price,
                        Quantity = cart.Quantity,
                        Notes = model.Notes,
                        FullName = model.FullName,
                        PhoneNumber = model.PhoneNumber,

                        // Lưu thông tin địa chỉ vào OrderDetails
                        Tinh = model.Tinh,
                        Quan = model.Quan,
                        Phuong = model.Phuong
                    };

                    var product = await _dataContext.Products.Where(p => p.Id == cart.ProductId).FirstAsync();
                    product.Quantity -= cart.Quantity;
                    product.Sold += cart.Quantity;

                    _dataContext.Update(product);
                    _dataContext.Add(orderDetails); // Lưu chi tiết đơn hàng vào cơ sở dữ liệu
                }

                await _dataContext.SaveChangesAsync(); // Lưu chi tiết đơn hàng

                HttpContext.Session.Remove("Cart");

                // Gửi mail xác nhận
                var receiver = userEmail;
                var subject = "Đặt hàng thành công!";
                var message = "Đặt hàng thành công, trải nghiệm dịch vụ vui vẻ.";
                await _emailSender.SendEmailAsync(receiver, subject, message);

                TempData["success"] = "Đặt hàng thành công. Vui lòng chờ duyệt đơn hàng!";
                return RedirectToAction("Index", "Cart");
            }
            else
            {
                TempData["error"] = "Vui lòng điền đầy đủ thông tin để tiến hành thanh toán.";
                return View(model);
            }
        }



        [HttpPost]
        public async Task<IActionResult> GetShipping(string tinh, string quan, string phuong)
        {
            var existingShipping = await _dataContext.Shippings
                .FirstOrDefaultAsync(x => x.City == tinh && x.District == quan && x.Ward == phuong);

            decimal shippingPrice = existingShipping?.Price ?? 50000; // Giá vận chuyển mặc định nếu không tìm thấy

            // Lưu giá vận chuyển vào cookies
            var shippingPriceJson = JsonConvert.SerializeObject(shippingPrice);
            try
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                    Secure = true
                };
                Response.Cookies.Append("ShippingPrice", shippingPriceJson, cookieOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while adding shipping price cookies: {ex.Message}");
            }

            return Json(new { shippingPrice });
        }



        [HttpGet]
        public IActionResult DeleteShipping()
        {
            // Kiểm tra xem cookie ShippingPrice có tồn tại không
            if (Request.Cookies["ShippingPrice"] != null)
            {
                // Xóa cookie ShippingPrice
                Response.Cookies.Delete("ShippingPrice");
                TempData["success"] = "Giá vận chuyển đã được xóa thành công!";
            }
            else
            {
                TempData["error"] = "Không có giá vận chuyển để xóa.";
            }

            // Điều hướng về trang Checkout
            return RedirectToAction("Checkout");
        }
        //Hàm lấy mã giảm giá
        [HttpPost]
        public async Task<IActionResult> GetCoupon(string coupon_value)
        {
            var validCoupon = await _dataContext.Coupons.FirstOrDefaultAsync(x => x.Name == coupon_value);

            if (validCoupon != null)
            {
                TimeSpan remainingTime = validCoupon.DateExpired - DateTime.Now;
                int daysRemaining = remainingTime.Days;

                if (daysRemaining >= 0)
                {
                    try
                    {
                        var cookiesOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                            Secure = true,
                            SameSite = SameSiteMode.Strict // kiểm tra tính tương thích trình duyệt
                        };

                        // Lưu mã giảm giá vào cookies
                        Response.Cookies.Append("CouponTitle", validCoupon.Name + " | " + validCoupon.Description, cookiesOptions);

                        return Ok(new { success = true, message = "Coupon applied successfully" });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error adding apply coupon cookies: {ex.Message}");
                        return Ok(new { success = false, message = "Coupon applied failed" });
                    }
                }
                else
                {
                    return Ok(new { success = false, message = "Coupon has expired" });
                }
            }
            else
            {
                return Ok(new { success = false, message = "Coupon not exist" });
            }
        }

    }
}