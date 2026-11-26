using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebDoThi.Data;
using WebDoThi.Models;
using System.Linq;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace WebDoThi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "1")]
    public class AdminUserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminUserController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var users = _context.TblUsers
                .Include(u => u.Role)
                .OrderByDescending(u => u.CreatedAt)
                .ToList();
            return View(users);
        }

        [HttpGet]
        public IActionResult Save(int? id)
        {
            ViewBag.Roles = _context.TblRoles.ToList();

            if (id == null || id == 0)
            {
                return View(new TblUser { Id = 0, Status = 1, TrustPoints = 0 });
            }

            var user = _context.TblUsers.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        public IActionResult Save(TblUser model, string newPassword)
        {
            if (model.Id == 0)
            {
                if (_context.TblUsers.Any(u => u.Email == model.Email))
                {
                    TempData["Error"] = "Email này đã tồn tại trong hệ thống!";
                    return RedirectToAction("Save", new { id = 0 });
                }

                model.CreatedAt = DateTime.Now;
                model.Status = 1;

                model.PasswordHash = newPassword ?? "123456";

                _context.TblUsers.Add(model);
            }
            else
            {
                var existingUser = _context.TblUsers.FirstOrDefault(u => u.Id == model.Id);
                if (existingUser != null)
                {
                    existingUser.FullName = model.FullName;
                    existingUser.Email = model.Email;
                    existingUser.RoleId = model.RoleId;
                    existingUser.TrustPoints = model.TrustPoints;

                    if (!string.IsNullOrEmpty(newPassword))
                    {
                        existingUser.PasswordHash = newPassword;
                    }
                }
            }

            _context.SaveChanges();
            TempData["Success"] = "Lưu thông tin tài khoản thành công!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ToggleStatus(int id)
        {
            var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int currentUserId = 0;
            int.TryParse(currentUserIdStr, out currentUserId);

            if (id == currentUserId)
            {
                TempData["Error"] = "Không thể khóa vì bạn đang đăng nhập vào tài khoản này!";
                return RedirectToAction("Index");
            }

            var user = _context.TblUsers.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                user.Status = (byte)(user.Status == 1 ? 0 : 1);
                _context.SaveChanges();
                TempData["Success"] = user.Status == 1 ? "Đã mở khóa tài khoản!" : "Đã khóa (xóa mềm) tài khoản!";
            }
            return RedirectToAction("Index");
        }
    }
}