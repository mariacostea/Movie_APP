namespace MobyLabWebProgramming.Core.DataTransferObjects;

public class MovieDetailsDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public int? Year { get; set; }
    public string? Description { get; set; }
    public string? PosterUrl { get; set; }
    public double AverageRating { get; set; }
    public List<GenreDTO> Genres { get; set; } = new();
}
