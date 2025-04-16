namespace MobyLabWebProgramming.Core.Entities
{
    public class Friendship : BaseEntity
    {
        public Guid RequesterId { get; set; }
        public User Requester { get; set; } = null!;
        
        public Guid AddresseeId { get; set; }
        public User Addressee { get; set; } = null!;
        
        public string Status { get; set; } = "Pending";
        
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? AcceptedAt { get; set; }
    }
}