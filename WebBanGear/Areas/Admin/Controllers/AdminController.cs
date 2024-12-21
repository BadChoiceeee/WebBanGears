using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBanGear.Repository;

namespace WebBanGear.Areas.Admin.Controllers
{
	[Authorize(Roles = "Admin,Manager")]
    public class AdminController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger, DataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }

        // Giả sử sau khi thực hiện quản lý hoặc thao tác gì đó, bạn muốn chuyển hướng về trang người dùng (trang hiển thị sản phẩm)
        public IActionResult ReturnToUI()
        {
            return RedirectToAction("Index", "Home"); // Chuyển về trang "Index" của HomeController
        }
        public IActionResult AccessDenied()
        {
            return View("/Home/Error?statuscode={0}"); // Trả về trang 404 nếu không có quyền
        }
    }

}
