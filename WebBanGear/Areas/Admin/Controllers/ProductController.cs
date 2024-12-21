using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanGear.Models;
using WebBanGear.Repository;

namespace WebBanGear.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = "Admin,Manager")]
    public class ProductController : Controller
	{
		private readonly DataContext _dataContext;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public ProductController(DataContext context, IWebHostEnvironment webHostEnvironment)
		{
			_dataContext = context;
			_webHostEnvironment = webHostEnvironment;
		}
		public async Task<IActionResult> Index()
		{
			return View(await _dataContext.Products.OrderByDescending(p => p.Id).Include(c => c.Category).Include(b => b.Brand).ToListAsync());
		}
		[HttpGet]
		public	IActionResult Create()
		{
			ViewBag.Categories = new SelectList(_dataContext.Categories,"Id","Name");
			ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name");
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(ProductModel product)
		{
			ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name",product.CategoryId);
			ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name",product.BrandID);

			if (ModelState.IsValid)
			{
				//code them dữ liệu
				product.Slug = product.Name.Replace(" ", "-");
				var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);
				if (slug != null)
				{
					ModelState.AddModelError("", "Sản phẩm đã có trong DataBase");
					return View(product);
				}
				if (product.ImageUpload != null)
				{
					string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
					string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
					string filePath = Path.Combine(uploadsDir, imageName);

					FileStream fs = new FileStream(filePath, FileMode.Create);
					await product.ImageUpload.CopyToAsync(fs);
					fs.Close();
					product.Image = imageName;
				}
				_dataContext.Add(product);
				await _dataContext.SaveChangesAsync();
				TempData["success"] = "Thêm sản phẩm thành công !";
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
            return View(product);
        }
		public async Task<IActionResult> Edit(long Id)
		{
			ProductModel product = await _dataContext.Products.FindAsync(Id);
			ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
			ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandID);
			return View(product);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(long Id, ProductModel product)
		{
			ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
			ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandID);

			if (ModelState.IsValid)
			{
				//code them dữ liệu
				product.Slug = product.Name.Replace(" ", "-");
                product.CapitalPrice = product.CapitalPrice;
                var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);
				if (slug != null)
				{
					ModelState.AddModelError("", "Sản phẩm đã có trong DataBase");
					return View(product);
				}
				if (product.ImageUpload != null)
				{
					string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
					string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
					string filePath = Path.Combine(uploadsDir, imageName);

					FileStream fs = new FileStream(filePath, FileMode.Create);
					await product.ImageUpload.CopyToAsync(fs);
					fs.Close();
					product.Image = imageName;
				}
				_dataContext.Update(product);
				await _dataContext.SaveChangesAsync();
				TempData["success"] = "Cập nhật sản phẩm thành công !";
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
		}
		public async Task<IActionResult> Delete(long Id)
		{
			ProductModel product = await _dataContext.Products.FindAsync(Id);
			if (!string.Equals(product.Image, "noname.jpg"))
			{
				string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
				string oldfileImage = Path.Combine(uploadsDir, product.Image);
				if (System.IO.File.Exists(oldfileImage))
				{
					System.IO.File.Delete(oldfileImage);
				}
			}
			_dataContext.Products.Remove(product); 
			await _dataContext.SaveChangesAsync();
			TempData["success"] = "Xoá sản phẩm thành công !";
			return RedirectToAction("Index");
		}
		// thêm số lượng spham vào product
		[HttpGet]
        public async Task<IActionResult> AddQuantity(int Id)
        {
			var productbyquantity = await _dataContext.ProductQuantities.Where(qp => qp.ProductId == Id).ToListAsync();
			ViewBag.ProductByQuantity = productbyquantity;
			ViewBag.Id = Id;
            return View();
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public  IActionResult StoreProductQuantity(ProductQuantityModel productQuantityModel)
		{
			var product = _dataContext.Products.Find(productQuantityModel.ProductId);
			if (product == null)
			{
				//return NotFound();
			}
			product.Quantity += productQuantityModel.Quantity;

			productQuantityModel.Quantity = productQuantityModel.Quantity;
			productQuantityModel.ProductId = productQuantityModel.ProductId;
			productQuantityModel.DateCreated = DateTime.Now;

			_dataContext.Add(productQuantityModel);
			_dataContext.SaveChangesAsync();
			TempData["success"] = "Thêm số lượng sản phẩm thành công !";
			return RedirectToAction("AddQuantity", "Product", new { Id = productQuantityModel.ProductId });
		}
	}
}
