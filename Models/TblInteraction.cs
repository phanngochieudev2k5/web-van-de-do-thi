using System;
using System.Collections.Generic;

namespace WebDoThi.Models;

public partial class TblInteraction
{
    public int Id { get; set; }

    public int ReportId { get; set; }

    public int UserId { get; set; }

    public string Type { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual TblReport Report { get; set; } = null!;

    public virtual TblUser User { get; set; } = null!;
}
