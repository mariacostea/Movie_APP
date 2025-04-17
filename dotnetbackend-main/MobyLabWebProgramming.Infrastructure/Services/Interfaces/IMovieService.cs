using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Responses;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;

public interface IMovieService
{
    Task<Movie> AddOrGetMovieFromApi(MovieDTO dto);

    Task<PagedResponse<MovieDetailsDTO>> GetAllMoviesAsync(int page, int pageSize);
    Task<MovieDetailsDTO?> GetMovieByTitleAsync(string title, int year);

    Task<PagedResponse<MovieDetailsDTO>> GetMoviesByGenreAsync(string genre, int page, int pageSize);
}