using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;
using MobyLabWebProgramming.Core.Configuration;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Core.Responses;
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

    public async Task<PagedResponse<MovieDetailsDTO>> GetAllMoviesAsync(int page, int pageSize)
    {
        var spec = new MovieProjectionSpec(page, pageSize);
        var countSpec = new MovieSpec();

        var movies = await _movieRepo.ListAsync(spec);
        var totalCount = await _movieRepo.GetCountAsync(countSpec);

        return new PagedResponse<MovieDetailsDTO>(page, pageSize, totalCount, movies);
    }

    public async Task<MovieDetailsDTO?> GetMovieByTitleAsync(string title)
    {
        var spec = new MovieProjectionSpec(title);
        return await _movieRepo.GetAsync(spec);
    }


    public async Task<PagedResponse<MovieDetailsDTO>> GetMoviesByGenreAsync(string genre, int page, int pageSize)
    {
        var spec = new MovieProjectionSpec(genre: genre, page: page, pageSize: pageSize);
        var movies = await _movieRepo.ListAsync(spec);
        var total = await _movieRepo.GetCountAsync(new MovieSpec(genre, isGenre: true));

        return new PagedResponse<MovieDetailsDTO>(page, pageSize, total, movies);
    }
}
