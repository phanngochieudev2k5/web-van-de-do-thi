using System;
using System.Collections.Generic;

namespace WebDoThi.Models;

public partial class TblReward
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int PointCost { get; set; }

    public int? StockQuantity { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<TblUserReward> TblUserRewards { get; set; } = new List<TblUserReward>();
}
