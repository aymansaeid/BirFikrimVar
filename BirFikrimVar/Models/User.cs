using System;
using System.Collections.Generic;

namespace BirFikrimVar.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Idea> Ideas { get; set; } = new List<Idea>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
}
