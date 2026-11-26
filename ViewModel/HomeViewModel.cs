using System.Collections.Generic;

namespace WebDoThi.Models
{
    public class HomeViewModel
    {
        public int TotalReports { get; set; }
        public int ResolvedReports { get; set; }
        public int ProcessingReports { get; set; }
        public int ActiveCitizens { get; set; }
        public List<TblCategory> Categories { get; set; }
        public List<ReportFeedItem> RecentReports { get; set; }
    }

    public class ReportFeedItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public string TimeAgo { get; set; }
        public string ImageUrl { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
    }
}