using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.DataTransferObjects;
using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
namespace MobyLabWebProgramming.Core.Specifications;

public sealed class MovieSpec : Specification<Movie>
{
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
        Query.Where(m => EF.Functions.ILike(m.Title, keyword) || EF.Functions.ILike(m.Description ?? "", keyword));
    }
}
