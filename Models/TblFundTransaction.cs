using System;
using System.Collections.Generic;

namespace WebDoThi.Models;

public partial class TblFundTransaction
{
    public int Id { get; set; }

    public int CampaignId { get; set; }

    public int? UserId { get; set; }

    public decimal Amount { get; set; }

    public string? OrderInfo { get; set; }

    public string? VnpayTranId { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual TblFundCampaign Campaign { get; set; } = null!;
}
