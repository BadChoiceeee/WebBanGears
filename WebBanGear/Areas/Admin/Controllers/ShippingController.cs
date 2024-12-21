using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanGear.Models;
using WebBanGear.Repository;

namespace WebBanGear.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Manager")]
    public class ShippingController : Controller
    {
        private readonly DataContext _dataContext;
        public ShippingController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index()
        {
            var shippingList = await _dataContext.Shippings.ToListAsync();
            ViewBag.Shippings = shippingList;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> StoreShipping(ShippingModel shippingModel, string phuong, string quan, string tinh, decimal price)
        {
            shippingModel.City = tinh;
            shippingModel.District = quan;
            shippingModel.Ward = phuong;
            shippingModel.Price = price;
            try
            {
                var existingShipping = await _dataContext.Shippings.AnyAsync(x => x.City == tinh && x.District == quan && x.Ward == phuong);

                //kiem tra neu data da co
                if (existingShipping)
                {
					TempData["error"] = "Dữ liệu trùng lặp.";
					return Ok(new { duplicate = true, message = "Dữ liệu trùng lặp."});
                }
                _dataContext.Shippings.Add(shippingModel);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm giá vận chuyển thành công!";
                return Ok(new { success = true, message = "Thêm giá vận chuyển thành công !" });
            }
            catch (Exception )
            {
                return StatusCode(500, "An error occured while adding shipping");
            }
        }
        public async Task<IActionResult> Delete(int Id)
        {
            ShippingModel shipping = await _dataContext.Shippings.FindAsync(Id);
            _dataContext.Shippings.Remove(shipping);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Xóa giá vận chuyển thành công!";
            return RedirectToAction("Index","Shipping");
        }
    }
}
