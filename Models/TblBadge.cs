using System;
using System.Collections.Generic;

namespace WebDoThi.Models;

public partial class TblBadge
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public int? RequiredReports { get; set; }

    public virtual ICollection<TblUserBadge> TblUserBadges { get; set; } = new List<TblUserBadge>();
}
