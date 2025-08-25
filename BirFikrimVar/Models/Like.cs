using System;
using System.Collections.Generic;

namespace BirFikrimVar.Models;

public partial class Like
{
    public int LikeId { get; set; }

    public int IdeaId { get; set; }

    public int UserId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Idea Idea { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
