using System;
using System.Collections.Generic;

namespace WebDoThi.Models
{
    public class ReportDetailViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CategoryName { get; set; }
        public string ReporterName { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string WardName { get; set; }
        public int? Severity { get; set; }
        public int ReporterId { get; set; }
        public int CurrentUserId { get; set; }
        public int LikesCount { get; set; }
        public List<string> Images { get; set; }
        public List<CommentItem> Comments { get; set; }
    }

    public class CommentItem
    {
        public string UserName { get; set; }
        public string Content { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}