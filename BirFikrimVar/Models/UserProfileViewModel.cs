namespace BirFikrimVar.Models
{
    public class UserProfileViewModel
    {
        public User User { get; set; } = null!;
        public List<Idea> Ideas { get; set; } = new List<Idea>();
    }
}
