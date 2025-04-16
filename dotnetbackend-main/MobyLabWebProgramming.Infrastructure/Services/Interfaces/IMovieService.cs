using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;

public interface IMovieService
{
    Task<Movie> AddOrGetMovieFromApi(MovieDTO dto);
    Task<List<MovieDetailsDTO>> GetAllMoviesAsync();
    Task<List<MovieDetailsDTO>> GetMoviesByTitleAsync(string title);
    Task<List<MovieDetailsDTO>> GetMoviesByGenreAsync(string genre);

}