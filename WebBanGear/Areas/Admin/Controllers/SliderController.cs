using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanGear.Models;
using WebBanGear.Repository;

namespace WebBanGear.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Manager")]
    public class SliderController : Controller
    {
        private readonly DataContext _dataContext;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public SliderController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = context;
			_webHostEnvironment = webHostEnvironment;
		}
        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Sliders.OrderByDescending(p => p.Id).ToListAsync());
        }
		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(SliderModel slider)
		{
		
			if (ModelState.IsValid)
			{
				//code them dữ liệu
				if (slider.ImageUpload != null)
				{
					string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/sliders");
					string imageName = Guid.NewGuid().ToString() + "_" + slider.ImageUpload.FileName;
					string filePath = Path.Combine(uploadsDir, imageName);

					FileStream fs = new FileStream(filePath, FileMode.Create);
					await slider.ImageUpload.CopyToAsync(fs);
					fs.Close();
					slider.Image = imageName;
				}
				_dataContext.Add(slider);
				await _dataContext.SaveChangesAsync();
				TempData["success"] = "Thêm Banner thành công !";
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
			return View(slider);
		}
		public async Task<IActionResult> Edit(int Id)
		{
			SliderModel sliders = await _dataContext.Sliders.FindAsync(Id);
			return View(sliders);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(SliderModel slider)
		{
			var  slider_existed = _dataContext.Sliders.Find(slider.Id);
			if (ModelState.IsValid)
			{
				//code them dữ liệu
				if (slider.ImageUpload != null)
				{
					string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/sliders");
					string imageName = Guid.NewGuid().ToString() + "_" + slider.ImageUpload.FileName;
					string filePath = Path.Combine(uploadsDir, imageName);

					FileStream fs = new FileStream(filePath, FileMode.Create);
					await slider.ImageUpload.CopyToAsync(fs);
					fs.Close();
					slider_existed.Image = imageName;
				}

				slider_existed.Name = slider.Name;
				slider_existed.Description = slider.Description;
				slider_existed.Status = slider.Status;


				_dataContext.Update(slider_existed);
				await _dataContext.SaveChangesAsync();
				TempData["success"] = "Thêm Banner thành công !";
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
			return View(slider);
		}

		public async Task<IActionResult> Delete(int Id)
		{
			SliderModel slider = await _dataContext.Sliders.FindAsync(Id);
			string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/sliders");
			string oldfileImage = Path.Combine(uploadsDir, slider.Image);
			if (System.IO.File.Exists(oldfileImage))
			{
				System.IO.File.Delete(oldfileImage);
			}
			_dataContext.Sliders.Remove(slider);
			await _dataContext.SaveChangesAsync();
			TempData["success"] = "Xoá Banner thành công !";
			return RedirectToAction("Index");
		}
	}
}
