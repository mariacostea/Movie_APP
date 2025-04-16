using Ardalis.Specification;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.DataTransferObjects;
using Microsoft.EntityFrameworkCore;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class MovieProjectionSpec : Specification<Movie, MovieDetailsDTO>
{
    public MovieProjectionSpec()
    {
        Query.Include(m => m.MovieGenres)
            .ThenInclude(mg => mg.Genre);

        Query.Select(m => new MovieDetailsDTO
        {
            Id = m.Id,
            Title = m.Title,
            Year = m.Year,
            Description = m.Description,
            AverageRating = m.AverageRating,
            PosterUrl = m.PosterUrl,
            Genres = m.MovieGenres.Select(g => new GenreDTO
            {
                Id = g.Genre.Id,
                Name = g.Genre.Name
            }).ToList()
        });
    }

    public MovieProjectionSpec(string title, bool searchByTitle) : this()
    {
        Query.Where(m => m.Title.ToLower().Contains(title.ToLower()));
    }

    public MovieProjectionSpec(string genreName) : this()
    {
        if (!string.IsNullOrWhiteSpace(genreName))
        {
            Query.Where(m => m.MovieGenres.Any(g => g.Genre.Name.ToLower() == genreName.ToLower()));
        }
    }

}