using System;
using System.Collections.Generic;

namespace WebDoThi.Models;

public partial class TblFundCampaign
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal TargetAmount { get; set; }

    public decimal? CurrentAmount { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<TblFundTransaction> TblFundTransactions { get; set; } = new List<TblFundTransaction>();
}
