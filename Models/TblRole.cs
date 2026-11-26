using System;
using System.Collections.Generic;

namespace WebDoThi.Models;

public partial class TblRole
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<TblMenu> TblMenus { get; set; } = new List<TblMenu>();

    public virtual ICollection<TblUser> TblUsers { get; set; } = new List<TblUser>();
}
