using System;
using System.Collections.Generic;

namespace WebDoThi.Models;

public partial class TblUser
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int? TrustPoints { get; set; }

    public byte? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual TblRole Role { get; set; } = null!;

    public virtual ICollection<TblComment> TblComments { get; set; } = new List<TblComment>();

    public virtual ICollection<TblInteraction> TblInteractions { get; set; } = new List<TblInteraction>();

    public virtual ICollection<TblReport> TblReports { get; set; } = new List<TblReport>();

    public virtual ICollection<TblUserBadge> TblUserBadges { get; set; } = new List<TblUserBadge>();

    public virtual ICollection<TblUserReward> TblUserRewards { get; set; } = new List<TblUserReward>();
}
