using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System;
using System.IO;
using System.Linq;
using WebDoThi.Data;
using WebDoThi.Models;

namespace WebDoThi.Controllers
{
    [Area("Admin")]
    [Authorize]
    [Route("Admin/News")]
    public class AdminNewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminNewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var newsList = _context.TblNews.OrderByDescending(n => n.CreatedAt).ToList();
            return View(newsList);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")]
        public IActionResult Create(string Title, string Content, IFormFile ImageFile)
        {
            if (string.IsNullOrWhiteSpace(Title)) Title = "Bản tin đô thị";
            if (string.IsNullOrWhiteSpace(Content)) Content = "Nội dung đang cập nhật...";

            var news = new TblNews
            {
                Title = Title,
                Content = Content.Replace("\n", "<br>"),
                CreatedAt = DateTime.Now,
                LikesCount = 0,
                CommentsCount = 0
            };

            if (ImageFile != null && ImageFile.Length > 0)
            {
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "news");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                string filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }

                news.ImageUrl = "/uploads/news/" + fileName;
            }

            _context.TblNews.Add(news);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}