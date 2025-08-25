using System;
using System.Collections.Generic;

namespace BirFikrimVar.Models;

public partial class Idea
{
    public int IdeaId { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public int? LikeCount { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual User User { get; set; } = null!;
}
