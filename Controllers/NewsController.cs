using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WebDoThi.Data;
using WebDoThi.Models;

namespace WebDoThi.Controllers
{
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var newsList = _context.TblNews.OrderByDescending(n => n.CreatedAt).ToList();
            return View(newsList);
        }

        public IActionResult Details(int id)
        {
            var newsItem = _context.TblNews
                .Include(n => n.TblNewsComments)
                .FirstOrDefault(n => n.Id == id);

            if (newsItem == null) return NotFound();
            return View(newsItem);
        }

        [HttpPost]
        public IActionResult AddComment([FromForm] int newsId, [FromForm] string content)
        {
            if (!User.Identity.IsAuthenticated)
                return Json(new { success = false, message = "Vui lòng đăng nhập" });

            if (string.IsNullOrWhiteSpace(content))
                return Json(new { success = false, message = "Bình luận không được để trống" });

            var comment = new TblNewsComment
            {
                NewsId = newsId,
                UserName = User.Identity.Name ?? "Người dùng ẩn danh",
                Content = content.Trim(),
                CreatedAt = DateTime.Now
            };

            _context.TblNewsComments.Add(comment);

            int currentCommentsCount = 0;
            var newsItem = _context.TblNews.Find(newsId);
            if (newsItem != null)
            {
                currentCommentsCount = (newsItem.CommentsCount ?? 0) + 1;
                newsItem.CommentsCount = currentCommentsCount;
            }

            _context.SaveChanges();

            return Json(new
            {
                success = true,
                userName = comment.UserName,
                content = comment.Content,
                time = comment.CreatedAt.Value.ToString("HH:mm - dd/MM/yyyy"),
                newCount = currentCommentsCount
            });
        }

        [HttpPost]
        public IActionResult ToggleLike([FromForm] int id, [FromForm] bool isLiking)
        {
            if (!User.Identity.IsAuthenticated)
                return Json(new { success = false, message = "Vui lòng đăng nhập" });

            var newsItem = _context.TblNews.Find(id);
            if (newsItem != null)
            {
                int currentLikes = newsItem.LikesCount ?? 0;
                newsItem.LikesCount = isLiking ? (currentLikes + 1) : Math.Max(0, currentLikes - 1);

                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Không tìm thấy bài viết" });
        }
    }
}