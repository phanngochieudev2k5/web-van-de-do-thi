using System;
using System.Collections.Generic;

namespace WebDoThi.Models;

public partial class TblCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Icon { get; set; }

    public virtual ICollection<TblReport> TblReports { get; set; } = new List<TblReport>();
}
