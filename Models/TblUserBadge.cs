using System;
using System.Collections.Generic;

namespace WebDoThi.Models;

public partial class TblUserBadge
{
    public int UserId { get; set; }

    public int BadgeId { get; set; }

    public DateTime? EarnedAt { get; set; }

    public virtual TblBadge Badge { get; set; } = null!;

    public virtual TblUser User { get; set; } = null!;
}
