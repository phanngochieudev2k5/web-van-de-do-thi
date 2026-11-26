using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WebDoThi.Data;
using WebDoThi.Models;

namespace WebDoThi.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, string returnUrl = null)
        {
            var user = _context.TblUsers.FirstOrDefault(u => u.Email == email && u.PasswordHash == password);


            if (user != null)
            {
                if (user.Status == 0)
                {
                    ViewBag.Error = "Tài khoản của bạn đã bị khóa!";
                    return View();
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.RoleId.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Email hoặc mật khẩu không chính xác!";
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(TblUser model)
        {
            if (_context.TblUsers.Any(u => u.Email == model.Email))
            {
                ViewBag.Error = "Địa chỉ Email này đã được sử dụng!";
                return View(model);
            }

            model.RoleId = 2;
            model.Status = 1;
            model.TrustPoints = 0;
            model.CreatedAt = DateTime.Now;

            _context.TblUsers.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}