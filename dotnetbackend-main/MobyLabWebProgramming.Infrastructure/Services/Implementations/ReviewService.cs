using System.Net;
using MobyLabWebProgramming.Core.Constants;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Enums;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Core.Specifications;
using MobyLabWebProgramming.Infrastructure.Database;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;
namespace MobyLabWebProgramming.Infrastructure.Services.Implementations;

public class ReviewService : IReviewService
{
    private readonly IRepository<WebAppDatabaseContext> _repo;

    public ReviewService(IRepository<WebAppDatabaseContext> repo)
    {
        _repo = repo;
    }

    public async Task AddReviewAsync(ReviewDTO dto, Guid userId)
    {
        var watchedSpec = new UserMovieSpec(userId, dto.MovieId);
        var hasWatched = await _repo.GetAsync(watchedSpec);

        if (hasWatched is null)
            throw new ServerException(HttpStatusCode.BadRequest, "You must watch the movie before leaving a review.");

        
        var review = new Review
        {
            UserId = userId,
            MovieId = dto.MovieId,
            Content = dto.Content,
            Rating = dto.Rating
        };

        await _repo.AddAsync(review);
        
        var allReviewsSpec = new ReviewSpec(dto.MovieId);
        var allReviews = await _repo.ListAsync(allReviewsSpec);

        var average = allReviews.Average(r => r.Rating);

        var movie = await _repo.GetAsync<Movie>(dto.MovieId);
        if (movie is not null)
        {
            movie.AverageRating = average;
            await _repo.UpdateAsync(movie);
        }
    }

    public async Task UpdateReviewAsync(Guid reviewId, ReviewDTO dto, Guid userId)
    {
        var review = await _repo.GetAsync<Review>(reviewId);

        if (review is null || review.UserId != userId)
            throw new ServerException(HttpStatusCode.NotFound, "Review not found or you're not authorized to update it.");

        review.Content = dto.Content;
        review.Rating = dto.Rating;

        await _repo.UpdateAsync(review);
        
        var allReviewsSpec = new ReviewSpec(dto.MovieId);
        var allReviews = await _repo.ListAsync(allReviewsSpec);
        var average = allReviews.Average(r => r.Rating);

        var movie = await _repo.GetAsync<Movie>(dto.MovieId);
        if (movie is not null)
        {
            movie.AverageRating = average;
            await _repo.UpdateAsync(movie);
        }
    }

    public async Task DeleteReviewAsync(Guid reviewId, Guid userId)
    {
        var review = await _repo.GetAsync<Review>(reviewId);

        if (review is null || review.UserId != userId)
            throw new ServerException(HttpStatusCode.NotFound, "Review not found or you're not authorized to delete it.");

        await _repo.DeleteAsync<Review>(reviewId);
        
        var allReviewsSpec = new ReviewSpec(review.MovieId);
        var allReviews = await _repo.ListAsync(allReviewsSpec);

        var movie = await _repo.GetAsync<Movie>(review.MovieId);
        if (movie is not null)
        {
            movie.AverageRating = allReviews.Any() ? allReviews.Average(r => r.Rating) : 0;
            await _repo.UpdateAsync(movie);
        }
    }

}
