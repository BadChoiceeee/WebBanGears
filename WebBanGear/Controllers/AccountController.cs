using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Claims;
using WebBanGear.Areas.Admin.Repository;
using WebBanGear.Models;
using WebBanGear.Models.ViewModels;
using WebBanGear.Repository;

namespace WebBanGear.Controllers
{
	public class AccountController : Controller
	{
		private UserManager<AppUserModel> _userManager;
		private SignInManager<AppUserModel> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly DataContext _dataContext;

        public AccountController(SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager, IEmailSender emailSender, DataContext context)
		{
			_signInManager = signInManager;
			_userManager = userManager;
            _emailSender = emailSender;
            _dataContext = context;
        }
		public IActionResult Login(string returnUrl)
		{
			return View(new LoginViewModel { ReturnUrl = returnUrl });
		}
        public async Task<IActionResult> NewPass(AppUserModel user,string token)
        {
			var checkuser = await _userManager.Users.Where(u => u.Email == user.Email).Where(u => u.Token == user.Token).FirstOrDefaultAsync();
			if(checkuser != null)
			{
				ViewBag.Email = checkuser.Email;
				ViewBag.Token = token;
			}
			else
			{
                TempData["error"] = "Không tìm thấy Email hoặc sai token !";
                return RedirectToAction("ForgetPass", "Account");
            }
            return View();
        }
        public async Task<IActionResult> ForgetPass( string returnUrl)
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SendMailForgotPass(AppUserModel user)
        {
            var checkMail = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
			if(checkMail == null)
			{
                TempData["error"] = "Không tìm thấy Email !";
                return RedirectToAction("ForgetPass", "Account");
            }
			else
			{
				string token = Guid.NewGuid().ToString();

                checkMail.Token = token;
				_dataContext.Update(checkMail);
				await _dataContext.SaveChangesAsync();

                var receiver = checkMail.Email;
                var subject = "Đặt lại mật khẩu cho :" + checkMail.Email;
                var message = "Bấm vào link để đổi mật khẩu: <a href='" + $"{Request.Scheme}://{Request.Host}/Account/NewPass?email={checkMail.Email}&token={token}'>Đặt lại mật khẩu</a>";


                await _emailSender.SendEmailAsync(receiver, subject, message);

            }
            TempData["success"] = "Một Email khôi phục mật khẩu đã được gửi tới Email mà bạn đăng ký ! ";
            return RedirectToAction("ForgetPass", "Account");
        }
        public async Task<IActionResult> UpdateNewPassword(AppUserModel user, string token)
        {
			var checkuser = await _userManager.Users.Where(u => u.Email == user.Email).Where(u => u.Token == user.Token).FirstOrDefaultAsync();
			if(checkuser != null)
			{
				//tạo chuỗi mới vào data
				string newtoken= Guid.NewGuid().ToString();
				//mã hóa mật khẩu mới
				var passwordHasher = new PasswordHasher<AppUserModel>();
				var passwordHash = passwordHasher.HashPassword(checkuser, user.PasswordHash);
				//gán mật khẩu mã hóa mới
				checkuser.PasswordHash = passwordHash;
				checkuser.Token = newtoken;

				await _userManager.UpdateAsync(checkuser);
                TempData["success"] = "Tạo mật khẩu mới thành công! ";
                return RedirectToAction("Login", "Account");
            }
			else
			{
                TempData["error"] = "Không tìm thấy Email hoặc Token không đúng !";
                return RedirectToAction("ForgetPass", "Account");
            }
        }

        [HttpPost]
		public async Task<IActionResult> Login(LoginViewModel loginVM)
		{
			if (ModelState.IsValid)
			{
				Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginVM.Username, loginVM.Password, false, false);
				if (result.Succeeded)
				{
                    TempData["success"] = "Đăng nhập thành công !";
                    return Redirect(loginVM.ReturnUrl ?? "/");
				}
				ModelState.AddModelError("", " UserName hoặc Password không đúng !");
			}
			return View(loginVM);
		}
		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Create(UserModel user)
		{
			var existingUser = await _userManager.FindByEmailAsync(user.Email);
			if (existingUser != null)
			{
				TempData["error"] = "Tạo User không thành công , có thể email hoặc Username đã được sử dụng";
				// Nếu địa chỉ email đã tồn tại, thêm lỗi vào ModelState và hiển thị lại trang tạo người dùng với thông báo lỗi
				return View(user);
			}
			if (ModelState.IsValid)
			{

				AppUserModel newUser = new AppUserModel { UserName = user.Username, Email = user.Email };
				IdentityResult result = await _userManager.CreateAsync(newUser, user.Password);

				if (result.Succeeded)
				{
					TempData["success"] = "Tạo User thành công !";
					return Redirect("/account/login");
				}
				foreach (IdentityError error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
			}
			return View(user);
		}
		public async Task<IActionResult> Logout(string returnUrl = "/")
		{
			await _signInManager.SignOutAsync();
			return Redirect(returnUrl);
		}
	}
}
