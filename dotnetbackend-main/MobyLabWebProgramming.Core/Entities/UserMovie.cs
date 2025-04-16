namespace MobyLabWebProgramming.Core.Entities
{
    public class UserMovie : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        
        public Guid MovieId { get; set; }
        
        public Movie Movie { get; set; } = null!;

        public bool IsWatched { get; set; }
        public DateTime? WatchedOn { get; set; }
        
    }
}