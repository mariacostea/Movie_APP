using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;
using MobyLabWebProgramming.Core.Configuration;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Core.Specifications;
using MobyLabWebProgramming.Infrastructure.Database;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;

namespace MobyLabWebProgramming.Infrastructure.Services.Implementations;

public class MovieService : IMovieService
{
    private readonly IRepository<WebAppDatabaseContext> _movieRepo;
    private readonly IOptions<TMDBConfiguration> _tmdbConfig;

    public MovieService(
        IRepository<WebAppDatabaseContext> movieRepo,
        IOptions<TMDBConfiguration> tmdbConfig)
    {
        _movieRepo = movieRepo;
        _tmdbConfig = tmdbConfig;
    }

    public async Task<Movie> AddOrGetMovieFromApi(MovieDTO dto)
    {
        var spec = new MovieByTitleAndYearSpec(dto.Title, dto.Year);
        var existing = await _movieRepo.GetAsync(spec);

        if (existing is not null)
            return existing;

        var tmdbMovie = await FetchMovieFromTmdb(dto.Title, dto.Year ?? 0);

        if (tmdbMovie is null)
        {
            throw new ServerException(HttpStatusCode.BadRequest, "Invalid movie: not found in TMDB.");
        }

        var newMovie = new Movie
        {
            Title = tmdbMovie.Value.GetProperty("title").GetString() ?? dto.Title,
            Year = dto.Year,
            Description = tmdbMovie.Value.GetProperty("overview").GetString() ?? "No description available."
        };

        await _movieRepo.AddAsync(newMovie);
        return newMovie;
    }

    private async Task<JsonElement?> FetchMovieFromTmdb(string title, int year)
    {
        var url = $"{_tmdbConfig.Value.BaseUrl}/search/movie?api_key={_tmdbConfig.Value.ApiKey}&query={Uri.EscapeDataString(title)}&year={year}";

        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var results = doc.RootElement.GetProperty("results");

        return results.GetArrayLength() > 0 ? results[0].Clone() : null;
    }

    public async Task<List<MovieDetailsDTO>> GetMoviesAsync()
    {
        return await _movieRepo.ListAsync(new MovieProjectionSpec());
    }

    public async Task<List<MovieDetailsDTO>> GetMoviesByTitleAsync(string title)
    {
        return await _movieRepo.ListAsync(new MovieProjectionSpec(title));
    }

    public async Task<List<MovieDetailsDTO>> GetMoviesByGenreAsync(string genre)
    {
        return await _movieRepo.ListAsync(new MovieProjectionSpec(genre));
    }
    public async Task<List<MovieDetailsDTO>> GetAllMoviesAsync()
    {
        return await _movieRepo.ListAsync(new MovieProjectionSpec());
    }

}
