namespace BirFikrimVar.Models
{
    public class LikeResponseDto
    {
        public int LikeId { get; set; }
        public int IdeaId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
    }
}
