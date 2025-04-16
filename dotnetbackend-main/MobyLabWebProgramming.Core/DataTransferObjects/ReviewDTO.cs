namespace MobyLabWebProgramming.Core.DataTransferObjects;

public class ReviewDTO
{
    public Guid MovieId { get; set; }
    public string Content { get; set; } = null!;
    public int Rating { get; set; }
}
