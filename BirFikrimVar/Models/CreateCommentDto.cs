namespace BirFikrimVar.Models
{
    public class CreateCommentDto
    {
        public int IdeaId { get; set; }   
        public int UserId { get; set; }   
        public string Content { get; set; } = null!;
    }
}
