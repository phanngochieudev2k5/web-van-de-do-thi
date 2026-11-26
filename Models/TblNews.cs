using System;
using System.Collections.Generic;

namespace WebDoThi.Models;

public partial class TblNews
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? LikesCount { get; set; }

    public int? CommentsCount { get; set; }

    public virtual ICollection<TblNewsComment> TblNewsComments { get; set; } = new List<TblNewsComment>();
}
