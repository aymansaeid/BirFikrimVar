using System;
using System.Collections.Generic;

namespace BirFikrimVar.Models;

public partial class Comment
{
    public int CommentId { get; set; }

    public int IdeaId { get; set; }

    public int UserId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public virtual Idea Idea { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
