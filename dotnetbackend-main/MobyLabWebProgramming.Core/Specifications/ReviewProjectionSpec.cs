using Ardalis.Specification;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class ReviewProjectionSpec : Specification<Review, ReviewDTO>
{
    public ReviewProjectionSpec(Guid movieId)
    {
        Query
            .Where(r => r.MovieId == movieId);
            
        Query   
            .Select(r => new ReviewDTO
            {
                MovieId = r.MovieId,
                Content = r.Content ?? "",
                Rating = r.Rating
            });
    }
}