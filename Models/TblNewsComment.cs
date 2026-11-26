using System;
using System.Collections.Generic;

namespace WebDoThi.Models;

public partial class TblNewsComment
{
    public int Id { get; set; }

    public int NewsId { get; set; }

    public string UserName { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual TblNews News { get; set; } = null!;
}
