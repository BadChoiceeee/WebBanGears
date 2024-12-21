using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanGear.Models;
using WebBanGear.Repository;

namespace WebBanGear.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = "Admin,Manager")]
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;
        public BrandController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index()
        {
			return View(await _dataContext.Brands.OrderByDescending(b => b.Id).ToArrayAsync());
		}
        public async Task<IActionResult> Create()
        {
            return View();
        }
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(BrandModel brand)
		{
			if (ModelState.IsValid)
			{
				//code them dữ liệu
				brand.Slug = brand.Name.Replace(" ", "-");
				var slug = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == brand.Slug);
				if (slug != null)
				{
					ModelState.AddModelError("", "Thương hiệu đã có trong DataBase");
					return View(brand);
				}
				_dataContext.Add(brand);
				await _dataContext.SaveChangesAsync();
				TempData["success"] = "Thêm thương hiệu thành công !";
				return RedirectToAction("Index");
			}
			else
			{
				TempData["error"] = "Model có một vài thứ đang bị lỗi";
				List<string> errors = new List<string>();
				foreach (var value in ModelState.Values)
				{
					foreach (var error in value.Errors)
					{
						errors.Add(error.ErrorMessage);
					}
				}
				string errorMessage = string.Join("\n", errors);
				return BadRequest(errorMessage);
			}
			return View(brand);
		}
		public async Task<IActionResult> Delete(int Id)
		{
			BrandModel brand = await _dataContext.Brands.FindAsync(Id);
			_dataContext.Brands.Remove(brand);
			await _dataContext.SaveChangesAsync();
			TempData["success"] = "Đã xoá thương hiệu thành công !";
			return RedirectToAction("Index");
		}
		public async Task<IActionResult> Edit(int Id)
		{
			BrandModel brand = await _dataContext.Brands.FindAsync(Id);
			return View(brand);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(BrandModel brand)
		{
			if (ModelState.IsValid)
			{
				//code them dữ liệu
				brand.Slug = brand.Name.Replace(" ", "-");
				var slug = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == brand.Slug);
				if (slug != null)
				{
					ModelState.AddModelError("", "Thương hiệu đã có trong DataBase");
					return View(brand);
				}
				_dataContext.Update(brand);
				await _dataContext.SaveChangesAsync();
				TempData["success"] = "Cập nhật thương hiệu thành công !";
				return RedirectToAction("Index");
			}
			else
			{
				TempData["error"] = "Model có một vài thứ đang bị lỗi";
				List<string> errors = new List<string>();
				foreach (var value in ModelState.Values)
				{
					foreach (var error in value.Errors)
					{
						errors.Add(error.ErrorMessage);
					}
				}
				string errorMessage = string.Join("\n", errors);
				return BadRequest(errorMessage);
			}
			return View(brand);
		}
	}
}
