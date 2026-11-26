using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebDoThi.Data;
using WebDoThi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace WebDoThi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? categoryId)
        {
            var vm = new HomeViewModel();

            vm.TotalReports = _context.TblReports.Count();
            vm.ResolvedReports = _context.TblReports.Count(r => r.Status == "RESOLVED");
            vm.ProcessingReports = _context.TblReports.Count(r => r.Status == "IN_PROGRESS" || r.Status == "PENDING");
            vm.ActiveCitizens = _context.TblUsers.Count(u => u.RoleId == 2);

            vm.Categories = _context.TblCategories.ToList();

            var query = _context.TblReports
                .Include(r => r.TblReportImages)
                .Include(r => r.TblInteractions)
                .Include(r => r.TblComments)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(r => r.CategoryId == categoryId.Value);
            }

            var rawReports = query.OrderByDescending(r => r.CreatedAt).Take(36).ToList();
            vm.RecentReports = new List<ReportFeedItem>();

            foreach (var r in rawReports)
            {
                var timeSpan = DateTime.Now - (r.CreatedAt ?? DateTime.Now);
                string timeStr;

                if (timeSpan.TotalMinutes < 1)
                    timeStr = "Vừa xong";
                else if (timeSpan.TotalMinutes < 60)
                    timeStr = (int)timeSpan.TotalMinutes + " phút trước";
                else if (timeSpan.TotalHours < 24)
                    timeStr = (int)timeSpan.TotalHours + " giờ trước";
                else
                    timeStr = r.CreatedAt?.ToString("dd/MM/yyyy HH:mm");

                vm.RecentReports.Add(new ReportFeedItem
                {
                    Id = r.Id,
                    Title = r.Title,
                    Address = r.Address,
                    Status = r.Status,
                    TimeAgo = timeStr,
                    ImageUrl = r.TblReportImages.Select(i => i.ImageUrl).FirstOrDefault() ?? "",
                    LikesCount = r.TblInteractions.Count(i => i.Type == "LIKE"),
                    CommentsCount = r.TblComments.Count
                });
            }

            ViewBag.ActiveCategoryId = categoryId;
            return View(vm);
        }

        private string GetWardName(string code)
        {
            if (string.IsNullOrEmpty(code)) return "";
            var dict = new Dictionary<string, string> {
                {"ben_thuy", "Phường Bến Thủy"}, {"cua_nam", "Phường Cửa Nam"}, {"doi_cung", "Phường Đội Cung"},
                {"dong_vinh", "Phường Đông Vĩnh"}, {"ha_huy_tap", "Phường Hà Huy Tập"}, {"hong_son", "Phường Hồng Sơn"},
                {"hung_binh", "Phường Hưng Bình"}, {"hung_dung", "Phường Hưng Dũng"}, {"hung_phuc", "Phường Hưng Phúc"},
                {"le_loi", "Phường Lê Lợi"}, {"le_mao", "Phường Lê Mao"}, {"quan_bau", "Phường Quán Bàu"},
                {"quang_trung", "Phường Quang Trung"}, {"trung_do", "Phường Trung Đô"}, {"truong_thi", "Phường Trường Thi"},
                {"vinh_tan", "Phường Vinh Tân"}, {"hung_chinh", "Xã Hưng Chính"}, {"hung_dong", "Xã Hưng Đông"},
                {"hung_hoa", "Xã Hưng Hòa"}, {"hung_loc", "Xã Hưng Lộc"}, {"nghi_an", "Xã Nghi Ân"},
                {"nghi_duc", "Xã Nghi Đức"}, {"nghi_kim", "Xã Nghi Kim"}, {"nghi_lien", "Xã Nghi Liên"},
                {"nghi_phu", "Xã Nghi Phú"}
            };
            return dict.ContainsKey(code) ? dict[code] : code;
        }

        public IActionResult Details(int id)
        {
            var report = _context.TblReports.FirstOrDefault(r => r.Id == id);
            if (report == null) return NotFound();

            var category = _context.TblCategories.FirstOrDefault(c => c.Id == report.CategoryId);
            var user = _context.TblUsers.FirstOrDefault(u => u.Id == report.UserId);
            var images = _context.TblReportImages.Where(i => i.ReportId == id).Select(i => i.ImageUrl).ToList();
            var likesCount = _context.TblInteractions.Count(i => i.ReportId == id && i.Type == "LIKE");

            var comments = _context.TblComments
                .Where(c => c.ReportId == id)
                .OrderByDescending(c => c.CreatedAt)
                .ToList()
                .Select(c => new CommentItem
                {
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    UserName = _context.TblUsers.FirstOrDefault(u => u.Id == c.UserId)?.FullName ?? "Ẩn danh"
                }).ToList();

            int currentUserId = 0;
            if (User.Identity.IsAuthenticated)
            {
                currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            }

            var vm = new ReportDetailViewModel
            {
                Id = report.Id,
                Title = report.Title,
                Description = report.Description,
                Address = report.Address,
                PhoneNumber = report.PhoneNumber,
                Status = report.Status,
                CreatedAt = report.CreatedAt,
                CategoryName = category?.Name ?? "Chưa phân loại",
                ReporterName = user?.FullName ?? "Công dân",
                Latitude = (double?)report.Latitude,
                Longitude = (double?)report.Longitude,
                WardName = GetWardName(report.WardCode),
                Severity = report.Severity,
                ReporterId = report.UserId,
                CurrentUserId = currentUserId,
                LikesCount = likesCount,
                Images = images,
                Comments = comments
            };

            return View(vm);
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddComment(int reportId, string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                var comment = new TblComment
                {
                    ReportId = reportId,
                    UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                    Content = content,
                    CreatedAt = DateTime.Now
                };
                _context.TblComments.Add(comment);
                _context.SaveChanges();
            }
            return RedirectToAction("Details", new { id = reportId });
        }

        [HttpGet]
        public IActionResult Map()
        {
            var reports = _context.TblReports
                .Where(r => r.Latitude != null && r.Longitude != null)
                .Select(r => new {
                    Id = r.Id,
                    Title = r.Title,
                    Address = r.Address,
                    Latitude = r.Latitude,
                    Longitude = r.Longitude,
                    Severity = r.Severity,
                    Status = r.Status,
                    ImageUrl = r.TblReportImages.Select(i => i.ImageUrl).FirstOrDefault() ?? "https://via.placeholder.com/300x200?text=Khong+co+anh"
                }).ToList();

            ViewBag.MapData = System.Text.Json.JsonSerializer.Serialize(reports);
            return View();
        }

        [Authorize]
        public IActionResult MyReports(int? categoryId, string status, DateTime? dateFilter)
        {
            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var query = _context.TblReports
                .Include(r => r.Category)
                .Include(r => r.TblReportImages)
                .Where(r => r.UserId == currentUserId)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(r => r.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(r => r.Status == status);
            }

            if (dateFilter.HasValue)
            {
                query = query.Where(r => r.CreatedAt.HasValue && r.CreatedAt.Value.Date == dateFilter.Value.Date);
            }

            ViewBag.Categories = _context.TblCategories.ToList();
            ViewBag.CurrentCategory = categoryId;
            ViewBag.CurrentStatus = status;
            ViewBag.CurrentDate = dateFilter?.ToString("yyyy-MM-dd");

            var reports = query.OrderByDescending(r => r.CreatedAt).ToList();
            return View(reports);
        }

        [HttpPost]
        public IActionResult LikeReport(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để thực hiện chức năng này!" });
            }

            var report = _context.TblReports.Find(id);
            if (report != null)
            {
                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var existingLike = _context.TblInteractions.FirstOrDefault(i => i.ReportId == id && i.UserId == currentUserId && i.Type == "LIKE");

                if (existingLike == null)
                {
                    _context.TblInteractions.Add(new TblInteraction
                    {
                        ReportId = id,
                        UserId = currentUserId,
                        Type = "LIKE",
                        CreatedAt = DateTime.Now
                    });
                    _context.SaveChanges();

                    int totalLikes = _context.TblInteractions.Count(i => i.ReportId == id && i.Type == "LIKE");

                    return Json(new { success = true, likes = totalLikes });
                }
                return Json(new { success = false, message = "Bạn đã ủng hộ báo cáo này rồi!" });
            }

            return Json(new { success = false, message = "Không tìm thấy dữ liệu báo cáo!" });
        }
    }
}