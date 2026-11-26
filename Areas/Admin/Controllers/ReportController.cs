using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using WebDoThi.Data;
using WebDoThi.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace WebDoThi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "1")]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string search, string status, byte? severity)
        {
            var query = _context.TblReports
                .Include(r => r.Category)
                .Include(r => r.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r => r.Title.Contains(search) || r.User.FullName.Contains(search) || r.Id.ToString() == search);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(r => r.Status == status);
            }

            if (severity.HasValue)
            {
                query = query.Where(r => r.Severity == severity.Value);
            }

            var reports = query.OrderByDescending(r => r.CreatedAt).ToList();

            ViewBag.Search = search;
            ViewBag.Status = status;
            ViewBag.Severity = severity;

            return View(reports);
        }

        public IActionResult Details(int id)
        {
            var report = _context.TblReports
                .Include(r => r.Category)
                .Include(r => r.User)
                .Include(r => r.TblReportImages)
                .Include(r => r.TblComments).ThenInclude(c => c.User)
                .FirstOrDefault(r => r.Id == id);

            if (report == null) return NotFound();

            return View(report);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, string status, string rejectReason, IFormFile resolvedImage)
        {
            var report = _context.TblReports.Find(id);
            if (report != null)
            {
                report.Status = status;
                report.UpdatedAt = DateTime.Now;

                if (status == "RESOLVED" || status == "REJECTED")
                {
                    report.ResolvedAt = DateTime.Now;
                }

                int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                if (status == "REJECTED" && !string.IsNullOrEmpty(rejectReason))
                {
                    _context.TblComments.Add(new TblComment
                    {
                        ReportId = id,
                        UserId = currentUserId,
                        Content = "LÝ DO TỪ CHỐI BÁO CÁO: " + rejectReason,
                        CreatedAt = DateTime.Now
                    });
                }

                if (status == "RESOLVED" && resolvedImage != null)
                {
                    string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "resolved");
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(resolvedImage.FileName);
                    string filePath = Path.Combine(uploadFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        resolvedImage.CopyTo(stream);
                    }

                    _context.TblComments.Add(new TblComment
                    {
                        ReportId = id,
                        UserId = currentUserId,
                        Content = $"Báo cáo đã được cơ quan chức năng xử lý hoàn tất. Đây là hình ảnh minh chứng thực tế: [IMG]/uploads/resolved/{fileName}[/IMG]",
                        CreatedAt = DateTime.Now
                    });
                }

                _context.SaveChanges();
            }
            return RedirectToAction("Details", new { id = id });
        }

        [HttpPost]
        public IActionResult AddOfficialComment(int reportId, string content)
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
        [HttpPost]
        public async Task<IActionResult> AskAdminAI(string message, int? reportId)
        {
            try
            {
                string reportContext = "";
                if (reportId.HasValue)
                {
                    var report = _context.TblReports.Find(reportId.Value);
                    if (report != null)
                    {
                        reportContext = $"THÔNG TIN BÁO CÁO (ID: {report.Id}):\n- Tiêu đề: {report.Title}\n- Địa chỉ: {report.Address}\n- Mô tả: {report.Description}\n- Mức độ: {report.Severity}\n";
                    }
                }

                string systemPrompt = @"Bạn là trợ lý AI thông minh chuyên hỗ trợ cán bộ quản lý hệ thống Đô thị thông minh SmartCitizen. Trả lời ngắn gọn, súc tích bằng tiếng Việt, tập trung vào việc tóm tắt hoặc soạn văn bản hành chính dựa trên thông tin được cung cấp.";
                string fullPrompt = systemPrompt + "\n\n" + reportContext + "\n\nYêu cầu: " + message;

                string apiKey = "AIzaSyDj0SqdcFj1ZhhdhHOGojOYxGm3U5jUVKE";

                if (string.IsNullOrEmpty(apiKey))
                {
                    return Json(new { reply = "⚠️ Báo cáo đồng chí: API Key đang bị trống!" });
                }

                string apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

                var payload = new
                {
                    contents = new[] { new { parts = new[] { new { text = fullPrompt } } } }
                };

                using (var client = new HttpClient())
                {
                    var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        using (System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(jsonResponse))
                        {
                            var root = doc.RootElement;
                            string aiReply = root.GetProperty("candidates")[0]
                                                 .GetProperty("content")
                                                 .GetProperty("parts")[0]
                                                 .GetProperty("text").GetString();

                            return Json(new { reply = aiReply });
                        }
                    }
                    else
                    {
                        string errorDetail = await response.Content.ReadAsStringAsync();
                        return Json(new { reply = $"⚠️ Google từ chối kết nối (Lỗi {response.StatusCode}). Xin hãy kiểm tra lại API Key. Chi tiết: {errorDetail}" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { reply = $"⚠️ Lỗi hệ thống C#: {ex.Message}" });
            }
        }
    }
}