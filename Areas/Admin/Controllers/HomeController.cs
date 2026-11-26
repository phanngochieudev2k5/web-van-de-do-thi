using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;
using WebDoThi.Data;

namespace WebDoThi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "1")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.Total = _context.TblReports.Count();
            ViewBag.Pending = _context.TblReports.Count(r => r.Status == "PENDING");
            ViewBag.Processing = _context.TblReports.Count(r => r.Status == "IN_PROGRESS");
            ViewBag.Resolved = _context.TblReports.Count(r => r.Status == "RESOLVED");

            var categoryStats = _context.TblReports
                .Where(r => r.Category != null)
                .GroupBy(r => r.Category.Name)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .ToList();
            ViewBag.CategoryStats = JsonSerializer.Serialize(categoryStats);

            var wardStats = _context.TblReports
                .Where(r => r.WardCode != null)
                .GroupBy(r => r.WardCode)
                .Select(g => new { Ward = g.Key, Count = g.Count() })
                .ToList();
            ViewBag.WardStats = JsonSerializer.Serialize(wardStats);

            var heatmapData = _context.TblReports
                .Where(r => r.Latitude != null && r.Longitude != null)
                .Select(r => new { r.Id, r.Title, r.Address, r.Latitude, r.Longitude, r.Severity })
                .ToList();
            ViewBag.HeatmapData = JsonSerializer.Serialize(heatmapData);

            return View();
        }
    }
}