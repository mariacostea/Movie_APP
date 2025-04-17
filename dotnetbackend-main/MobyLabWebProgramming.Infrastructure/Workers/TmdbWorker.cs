using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MobyLabWebProgramming.Core.Configuration;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Specifications;
using MobyLabWebProgramming.Infrastructure.Database;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;

namespace MobyLabWebProgramming.Infrastructure.Workers;

public class TmdbImportWorker(
    ILogger<TmdbImportWorker> logger,
    IServiceScopeFactory scopeFactory,
    IOptions<TMDBConfiguration> tmdbConfig)
    : BackgroundService
{
    private int _currentPage = 500;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    const int pagesToFetchPerCycle = 0;

    while (!stoppingToken.IsCancellationRequested)
    {
        using var scope = scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IRepository<WebAppDatabaseContext>>();

        using var client = new HttpClient();

        for (int i = 0; i < pagesToFetchPerCycle; i++)
        {
            int currentPage = _currentPage + i;
            logger.LogInformation("[TMDB Worker] Fetching page {Page}", currentPage);

            var url = $"{tmdbConfig.Value.BaseUrl}/movie/popular?api_key={tmdbConfig.Value.ApiKey}&page={currentPage}";
            var response = await client.GetAsync(url, stoppingToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("[TMDB Worker] TMDB returned error for page {Page}", currentPage);
                continue;
            }

            var json = await response.Content.ReadAsStringAsync(stoppingToken);
            using var doc = JsonDocument.Parse(json);

            var movies = doc.RootElement.GetProperty("results").EnumerateArray();
            int added = 0;

            foreach (var movieJson in movies)
            {

                var title = movieJson.GetProperty("title").GetString();
                var year = movieJson.GetProperty("release_date").GetString()?.Split('-')[0];
                var description = movieJson.GetProperty("overview").GetString();
                var posterPath = movieJson.GetProperty("poster_path").GetString();
                var genreIds = movieJson.GetProperty("genre_ids").EnumerateArray().Select(e => e.GetInt32()).ToList();

                if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(year)) continue;
                if (!int.TryParse(year, out var parsedYear)) continue;

                if (await repo.GetAsync(new MovieByTitleAndYearSpec(title, parsedYear)) != null) continue;

                var movie = new Movie
                {
                    Title = title,
                    Year = parsedYear,
                    Description = description,
                    PosterUrl = !string.IsNullOrWhiteSpace(posterPath) ? $"https://image.tmdb.org/t/p/w500{posterPath}" : null
                };

                await repo.AddAsync(movie);
                added++;

                foreach (var genreId in genreIds)
                {
                    var genreName = GenreIdToName(genreId);
                    if (string.IsNullOrWhiteSpace(genreName)) continue;

                    var genre = await repo.GetAsync(new GenreSpec(genreName));
                    if (genre is null)
                    {
                        genre = new Genre { Name = genreName };
                        await repo.AddAsync(genre);
                    }

                    await repo.AddAsync(new MovieGenre
                    {
                        MovieId = movie.Id,
                        GenreId = genre.Id
                    });
                }
            }

            logger.LogInformation("[TMDB Worker] Added {Count} movies from page {Page}", added, currentPage);
        }

        _currentPage += pagesToFetchPerCycle;

        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
    }
}


    private static string? GenreIdToName(int id) => id switch
    {
        28 => "Action",
        12 => "Adventure",
        16 => "Animation",
        35 => "Comedy",
        80 => "Crime",
        99 => "Documentary",
        18 => "Drama",
        10751 => "Family",
        14 => "Fantasy",
        36 => "History",
        27 => "Horror",
        10402 => "Music",
        9648 => "Mystery",
        10749 => "Romance",
        878 => "Science Fiction",
        10770 => "TV Movie",
        53 => "Thriller",
        10752 => "War",
        37 => "Western",
        _ => null
    };
}
