using Ardalis.Specification;
using MobyLabWebProgramming.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class MovieSpec : Specification<Movie>
{
    public MovieSpec()
    {
    }
    
    public MovieSpec(Guid genreId)
    {
        Query.Where(m => m.MovieGenres.Any(mg => mg.GenreId == genreId));
    }
    
    public MovieSpec(int year)
    {
        Query.Where(m => m.Year == year);
    }
    
    public MovieSpec(string keyword)
    {
        keyword = $"%{keyword.Replace(" ", "%")}%";
        Query.Where(m =>
            EF.Functions.ILike(m.Title, keyword) ||
            EF.Functions.ILike(m.Description ?? "", keyword));
    }
    
    public MovieSpec(string title, bool isTitle)
    {
        if (isTitle && !string.IsNullOrWhiteSpace(title))
        {
            Query.Where(m => m.Title.ToLower().Contains(title.ToLower()));
        }
    }
    
    public MovieSpec(string genreName, bool isGenre, bool _ = true)
    {
        if (isGenre && !string.IsNullOrWhiteSpace(genreName))
        {
            Query.Where(m => m.MovieGenres.Any(g =>
                g.Genre.Name.ToLower() == genreName.ToLower()));
        }
    }
}