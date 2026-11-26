using System;
using System.Collections.Generic;

namespace WebDoThi.Models;

public partial class TblComment
{
    public int Id { get; set; }

    public int ReportId { get; set; }

    public int UserId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual TblReport Report { get; set; } = null!;

    public virtual TblUser User { get; set; } = null!;
}
