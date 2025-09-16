namespace BirFikrimVar.Models
{
    public class UserProfileDto
    {
        public UserDto User { get; set; } = null!;
        public List<IdeaDto> Ideas { get; set; } = new List<IdeaDto>();
    }
}
