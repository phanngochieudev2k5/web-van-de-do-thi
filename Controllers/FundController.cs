using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using WebDoThi.Data;
using WebDoThi.Models;
using WebDoThi.Services;

namespace WebDoThi.Controllers
{
    public class FundController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public FundController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index(string sort = "newest")
        {
            var query = _context.TblFundCampaigns
                                 .Where(c => c.Status == "ACTIVE" && c.EndDate >= DateTime.Now);

            if (sort == "highest")
            {
                query = query.OrderByDescending(c => c.CurrentAmount);
            }
            else if (sort == "oldest")
            {
                query = query.OrderBy(c => c.Id);
            }
            else
            {
                query = query.OrderByDescending(c => c.Id);
            }

            ViewBag.CurrentSort = sort;
            return View(query.ToList());
        }

        public IActionResult Details(int id)
        {
            var campaign = _context.TblFundCampaigns.FirstOrDefault(c => c.Id == id);
            if (campaign == null) return NotFound();

            ViewBag.RecentDonations = _context.TblFundTransactions
                                              .Where(t => t.CampaignId == id && t.Status == "SUCCESS")
                                              .OrderByDescending(t => t.CreatedAt)
                                              .Take(10)
                                              .ToList();
            return View(campaign);
        }

        [HttpPost]
        [Authorize]
        public IActionResult CreatePayment(int campaignId, decimal amount, string message)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var transaction = new TblFundTransaction
            {
                CampaignId = campaignId,
                UserId = userId,
                Amount = amount,
                OrderInfo = string.IsNullOrEmpty(message) ? "Quyen gop chien dich " + campaignId : message,
                Status = "PENDING",
                CreatedAt = DateTime.Now
            };
            _context.TblFundTransactions.Add(transaction);
            _context.SaveChanges();

            string vnp_TmnCode = _configuration["Vnpay:TmnCode"];
            string vnp_HashSecret = _configuration["Vnpay:HashSecret"];
            string vnp_Url = _configuration["Vnpay:BaseUrl"];
            string vnp_Returnurl = _configuration["Vnpay:ReturnUrl"];

            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (amount * 100).ToString("0"));
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1");
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", transaction.OrderInfo);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", transaction.Id.ToString());

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return Redirect(paymentUrl);
        }

        [HttpGet]
        public IActionResult PaymentCallback()
        {
            var vnpay = new VnPayLibrary();
            var responseData = HttpContext.Request.Query;

            foreach (var (key, value) in responseData)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            int txnId = Convert.ToInt32(vnpay.GetResponseData("vnp_TxnRef"));
            string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            string vnp_SecureHash = responseData["vnp_SecureHash"];
            string hashSecret = _configuration["Vnpay:HashSecret"];

            var transaction = _context.TblFundTransactions.FirstOrDefault(t => t.Id == txnId);
            if (transaction != null && transaction.Status == "PENDING")
            {
                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, hashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        transaction.Status = "SUCCESS";
                        transaction.VnpayTranId = vnpay.GetResponseData("vnp_TransactionNo");

                        var campaign = _context.TblFundCampaigns.Find(transaction.CampaignId);
                        if (campaign != null)
                        {
                            campaign.CurrentAmount = (campaign.CurrentAmount ?? 0) + transaction.Amount;
                        }
                        ViewBag.Message = "Cảm ơn bạn! Giao dịch quyên góp thành công.";
                    }
                    else
                    {
                        transaction.Status = "FAILED";
                        ViewBag.Message = "Giao dịch đã bị hủy hoặc xảy ra lỗi từ phía ngân hàng.";
                    }
                }
                else
                {
                    transaction.Status = "FAILED";
                    ViewBag.Message = "Lỗi bảo mật: Dữ liệu giao dịch có dấu hiệu bị can thiệp!";
                }
                _context.SaveChanges();
            }

            return View();
        }
    }
}