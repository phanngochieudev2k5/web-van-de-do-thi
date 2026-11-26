using System;
using System.Collections.Generic;

namespace WebDoThi.Models;

public partial class TblUserReward
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int RewardId { get; set; }

    public DateTime? RedeemedAt { get; set; }

    public string VoucherCode { get; set; } = null!;

    public virtual TblReward Reward { get; set; } = null!;

    public virtual TblUser User { get; set; } = null!;
}
