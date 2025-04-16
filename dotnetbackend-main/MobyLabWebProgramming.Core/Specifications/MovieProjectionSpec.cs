using Ardalis.Specification;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Specifications;

using Microsoft.EntityFrameworkCore;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class MovieProjectionSpec : Specification<Movie, MovieDetailsDTO>
{
    public MovieProjectionSpec(int page = 1, int pageSize = 50)
    {
        Query.Include(m => m.MovieGenres)
            .ThenInclude(mg => mg.Genre)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        Query.Select(m => new MovieDetailsDTO
        {
            Id = m.Id,
            Title = m.Title,
            Year = m.Year,
            Description = m.Description,
            AverageRating = m.AverageRating,
            PosterUrl = m.PosterUrl,
            Genres = m.MovieGenres.Select(g => g.Genre.Name).ToList()
        });
    }

    public MovieProjectionSpec(string title, int page = 1, int pageSize = 50, bool filterByTitle = true)
        : this(page, pageSize)
    {
        if (!string.IsNullOrWhiteSpace(title))
        {
            Query.Where(m => m.Title.ToLower().Contains(title.ToLower()));
        }
    }
    
    public MovieProjectionSpec(string exactTitle)
    {
        Query.Where(m => m.Title.ToLower() == exactTitle.ToLower());
        Query.Select(m => new MovieDetailsDTO
        {
            Id = m.Id,
            Title = m.Title,
            Year = m.Year,
            Description = m.Description,
            AverageRating = m.AverageRating,
            PosterUrl = m.PosterUrl,
            Genres = m.MovieGenres.Select(g => g.Genre.Name).ToList()
        });
    }


    public MovieProjectionSpec(string genre, int page = 1, int pageSize = 50)
        : this(page, pageSize)
    {
        if (!string.IsNullOrWhiteSpace(genre))
        {
            Query.Where(m => m.MovieGenres.Any(g => g.Genre.Name.ToLower() == genre.ToLower()));
        }
    }
}