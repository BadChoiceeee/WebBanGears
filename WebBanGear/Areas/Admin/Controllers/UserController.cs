using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanGear.Models;
using WebBanGear.Repository;

namespace WebBanGear.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _dataContext;

        public UserController(DataContext context, UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dataContext = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {

            // Lấy danh sách người dùng và vai trò
            var usersWithRoles = await (from u in _dataContext.Users
                                        join ur in _dataContext.UserRoles on u.Id equals ur.UserId
                                        join r in _dataContext.Roles on ur.RoleId equals r.Id
                                        select new { User = u, RoleName = r.Name }).ToListAsync();

            return View(usersWithRoles);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(new AppUserModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppUserModel user)
        {
            if (ModelState.IsValid)
            {
                var createUserResult = await _userManager.CreateAsync(user, user.PasswordHash);//tạo user
                if (createUserResult.Succeeded)
                {
                    var createUser = await _userManager.FindByEmailAsync(user.Email);// tìm user dựa vào email
                    var userId = createUser.Id;// lấy user id
                    var role = _roleManager.FindByIdAsync(user.RoleId);//lấy role id
                    //gán quyền
                    var addToRoleResult = await _userManager.AddToRoleAsync(createUser, role.Result.Name);
                    if (!addToRoleResult.Succeeded)
                    {
                        foreach (var error in createUserResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    foreach (var error in createUserResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(user);
                }
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
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(user);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Xóa dữ liệu liên quan trong bảng Wishlists
            var wishlists = _dataContext.Wishlists.Where(w => w.UserId == id).ToList();
            if (wishlists.Any())
            {
                _dataContext.Wishlists.RemoveRange(wishlists);
                await _dataContext.SaveChangesAsync();
            }

            // Xóa người dùng
            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                TempData["error"] = "Xóa người dùng thất bại.";
                return View("Error");
            }

            TempData["success"] = "Đã xóa người dùng và dữ liệu liên quan thành công!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, AppUserModel user)
        {
            var existingUser = await _userManager.FindByIdAsync(id); // Lấy user dựa vào id
            if (existingUser == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Cập nhật thông tin người dùng
                existingUser.UserName = user.UserName;
                existingUser.Email = user.Email;
                existingUser.PhoneNumber = user.PhoneNumber;

                // Lấy danh sách các vai trò hiện tại của người dùng
                var currentRoles = await _userManager.GetRolesAsync(existingUser);

                // Xóa tất cả vai trò hiện tại của người dùng
                var removeRoleResult = await _userManager.RemoveFromRolesAsync(existingUser, currentRoles);
                if (!removeRoleResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Không thể xóa vai trò hiện tại của người dùng.");
                    return View(existingUser);
                }

                // Lấy tên vai trò từ RoleId
                var role = await _roleManager.FindByIdAsync(user.RoleId);
                if (role != null)
                {
                    // Thêm vai trò mới cho người dùng
                    var addRoleResult = await _userManager.AddToRoleAsync(existingUser, role.Name);
                    if (!addRoleResult.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Không thể thêm vai trò mới cho người dùng.");
                        return View(existingUser);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Vai trò không hợp lệ.");
                    return View(existingUser);
                }

                // Lưu thay đổi thông tin người dùng
                var updateUserResult = await _userManager.UpdateAsync(existingUser);
                if (updateUserResult.Succeeded)
                {
                    TempData["success"] = "Cập nhật người dùng thành công!";
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    foreach (var error in updateUserResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(existingUser);
                }
            }

            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(existingUser);
        }
    }
}
