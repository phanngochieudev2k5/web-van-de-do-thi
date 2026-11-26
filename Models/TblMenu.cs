using System;
using System.Collections.Generic;

namespace WebDoThi.Models;

public partial class TblMenu
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Url { get; set; } = null!;

    public string? Icon { get; set; }

    public int? ParentId { get; set; }

    public int? DisplayOrder { get; set; }

    public int? RequiredRoleId { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<TblMenu> InverseParent { get; set; } = new List<TblMenu>();

    public virtual TblMenu? Parent { get; set; }

    public virtual TblRole? RequiredRole { get; set; }
}
