using System;
using System.Collections.Generic;

namespace WebDoThi.Models;

public partial class TblDepartment
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? ContactEmail { get; set; }

    public virtual ICollection<TblReport> TblReports { get; set; } = new List<TblReport>();
}
