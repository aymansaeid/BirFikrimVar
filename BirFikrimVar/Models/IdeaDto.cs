namespace BirFikrimVar.Models
{
    public class IdeaDto
    {
        public int IdeaId { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public DateTime? CreatedDate { get; set; }

        public int? LikeCount { get; set; }
    }
}
