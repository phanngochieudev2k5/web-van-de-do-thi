using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System;
using System.IO;
using System.Linq;
using WebDoThi.Data;
using WebDoThi.Models;

namespace WebDoThi.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(TblReport model, IFormFile[] images)
        {
            if (string.IsNullOrWhiteSpace(model.Title)) model.Title = "Chưa có tiêu đề";
            if (string.IsNullOrWhiteSpace(model.Description)) model.Description = "Chưa có mô tả";
            if (string.IsNullOrWhiteSpace(model.Address)) model.Address = "Vị trí định vị qua GPS";

            var latStr = Request.Form["Lat"].ToString();
            var lngStr = Request.Form["Lng"].ToString();

            if (!string.IsNullOrEmpty(latStr) && !string.IsNullOrEmpty(lngStr))
            {
                model.Latitude = decimal.Parse(latStr.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                model.Longitude = decimal.Parse(lngStr.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
            }

            model.UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            model.Status = "PENDING";
            model.CreatedAt = DateTime.Now;
            model.UpdatedAt = DateTime.Now;

            _context.TblReports.Add(model);
            _context.SaveChanges();

            if (images != null && images.Length > 0)
            {
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "reports");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                foreach (var file in images)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    _context.TblReportImages.Add(new TblReportImage
                    {
                        ReportId = model.Id,
                        ImageUrl = "/uploads/reports/" + fileName
                    });
                }
                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var report = _context.TblReports.FirstOrDefault(r => r.Id == id && r.UserId == currentUserId && r.Status == "PENDING");

            if (report == null)
            {
                return RedirectToAction("MyReports", "Home");
            }

            ViewBag.Categories = _context.TblCategories.ToList();
            return View(report);
        }

        [HttpPost]
        public IActionResult Edit(int id, TblReport model)
        {
            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var report = _context.TblReports.FirstOrDefault(r => r.Id == id && r.UserId == currentUserId && r.Status == "PENDING");

            if (report != null)
            {
                report.Title = model.Title;
                report.Address = model.Address;
                report.Description = model.Description;
                report.PhoneNumber = model.PhoneNumber;
                report.CategoryId = model.CategoryId;
                report.Severity = model.Severity;
                report.UpdatedAt = DateTime.Now;

                _context.SaveChanges();
            }

            return RedirectToAction("Details", "Home", new { id = id });
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var report = _context.TblReports.FirstOrDefault(r => r.Id == id && r.UserId == currentUserId && r.Status == "PENDING");

            if (report != null)
            {
                var images = _context.TblReportImages.Where(i => i.ReportId == id).ToList();
                _context.TblReportImages.RemoveRange(images);

                var comments = _context.TblComments.Where(c => c.ReportId == id).ToList();
                _context.TblComments.RemoveRange(comments);

                _context.TblReports.Remove(report);
                _context.SaveChanges();
            }

            return RedirectToAction("MyReports", "Home");
        }
    }
}