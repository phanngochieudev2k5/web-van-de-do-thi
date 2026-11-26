using System;
using System.Collections.Generic;

namespace WebDoThi.Models;

public partial class TblReportImage
{
    public int Id { get; set; }

    public int ReportId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public virtual TblReport Report { get; set; } = null!;
}
