using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanGear.Repository;

namespace WebBanGear.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Manager")]
    public class DashboardController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DashboardController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var count_product = _dataContext.Products.Count();
            var count_order = _dataContext.Orders.Count();
            var count_category = _dataContext.Categories.Count();
            var count_user = _dataContext.Users.Count();
            ViewBag.CountProduct = count_product;
            ViewBag.CountOrder = count_order;
            ViewBag.CountCategory = count_category;
            ViewBag.CountUser = count_user;
            return View();
        }

        [HttpPost]
        [Route("GetChartData")]
        public async Task<IActionResult> GetChartData()
        {
            // Đảm bảo sử dụng await để gọi ToListAsync
            var data = await _dataContext.Statistticals
                .Select(s => new
                {
                    date = s.DateCreated.ToString("yyyy-MM-dd"),
                    sold = s.Sold,
                    quantity = s.Quantity,
                    revenue = s.Revenue,
                    profit = s.Profit
                })
                .ToListAsync();  // Đảm bảo là asynchronous và đợi kết quả

            return Json(data);  // Trả về dữ liệu dưới dạng JSON
        }

        [HttpPost]
        [Route("GetChartDataBySelect")]
        public async Task<IActionResult> GetChartDataBySelect(DateTime startDate, DateTime endDate)
        {
            var data = await  _dataContext.Statistticals.Where(s => s.DateCreated >= startDate && s.DateCreated <= endDate)
                .Select(s => new
                {
                    date = s.DateCreated.ToString("yyyy-MM-dd"),
                    sold = s.Sold,
                    quantity = s.Quantity,
                    revenue = s.Revenue,
                    profit = s.Profit
                })
                .ToListAsync();  // Đảm bảo là asynchronous và đợi kết quả

            return Json(data);  // Trả về dữ liệu dưới dạng JSON
        }

    }
}