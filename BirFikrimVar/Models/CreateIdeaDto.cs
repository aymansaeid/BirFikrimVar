namespace BirFikrimVar.Models
{
    public class CreateIdeaDto
    {
        public int UserId { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
    }
}
