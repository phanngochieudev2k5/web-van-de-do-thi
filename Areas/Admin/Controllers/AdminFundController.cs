using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using WebDoThi.Data;
using WebDoThi.Models;

namespace WebDoThi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    [Route("Admin/Fund")]
    public class AdminFundController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminFundController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var campaigns = _context.TblFundCampaigns.OrderByDescending(c => c.Id).ToList();

            ViewBag.TotalTarget = campaigns.Sum(c => c.TargetAmount);
            ViewBag.TotalRaised = campaigns.Sum(c => c.CurrentAmount ?? 0);
            ViewBag.TotalCampaigns = campaigns.Count;

            return View(campaigns);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")]
        public IActionResult Create(TblFundCampaign model)
        {
            model.CurrentAmount = 0;
            model.Status = "ACTIVE";

            if (model.StartDate == null) model.StartDate = DateTime.Now;
            if (model.EndDate == null) model.EndDate = DateTime.Now.AddDays(30);

            _context.TblFundCampaigns.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost("CloseCampaign")]
        public IActionResult CloseCampaign(int id)
        {
            var campaign = _context.TblFundCampaigns.Find(id);
            if (campaign != null)
            {
                campaign.Status = "CLOSED";
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpGet("Details/{id}")]
        public IActionResult Details(int id, string sort = "newest")
        {
            var campaign = _context.TblFundCampaigns.Find(id);
            if (campaign == null) return NotFound();

            var query = _context.TblFundTransactions
                .Where(t => t.CampaignId == id && t.Status == "SUCCESS");

            if (sort == "highest")
            {
                query = query.OrderByDescending(t => t.Amount);
            }
            else
            {
                query = query.OrderByDescending(t => t.CreatedAt);
            }

            var transactions = query.ToList();
            var users = _context.TblUsers.ToList();

            var transactionDetails = (from t in transactions
                                      join u in users on t.UserId equals u.Id into tu
                                      from u in tu.DefaultIfEmpty()
                                      select new
                                      {
                                          CreatedAt = t.CreatedAt,
                                          Amount = t.Amount,
                                          OrderInfo = t.OrderInfo,
                                          VnpayTranId = t.VnpayTranId,
                                          UserName = u != null ? u.FullName : "Ẩn danh"
                                      }).ToList();

            ViewBag.Transactions = transactionDetails;
            ViewBag.CurrentSort = sort;

            return View(campaign);
        }
    }
}