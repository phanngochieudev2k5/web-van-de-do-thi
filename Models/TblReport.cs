using System;
using System.Collections.Generic;

namespace WebDoThi.Models;

public partial class TblReport
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int CategoryId { get; set; }

    public int? DepartmentId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public byte? Severity { get; set; }

    public string? Status { get; set; }

    public string? WardCode { get; set; }

    public string? Address { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public string? PhoneNumber { get; set; }

    public virtual TblCategory Category { get; set; } = null!;

    public virtual TblDepartment? Department { get; set; }

    public virtual ICollection<TblComment> TblComments { get; set; } = new List<TblComment>();

    public virtual ICollection<TblInteraction> TblInteractions { get; set; } = new List<TblInteraction>();

    public virtual ICollection<TblReportImage> TblReportImages { get; set; } = new List<TblReportImage>();

    public virtual TblUser User { get; set; } = null!;
}
