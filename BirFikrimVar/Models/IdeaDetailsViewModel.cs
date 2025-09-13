namespace BirFikrimVar.Models
{
    public class IdeaDetailsViewModel
    {
        public IdeaDto Idea { get; set; }
        public List<CommentDto> Comments { get; set; } = new List<CommentDto>();
        public int LikeCount { get; set; }
        public bool UserLiked { get; set; }
    }
}
