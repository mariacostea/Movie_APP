using MobyLabWebProgramming.Core.Configuration;
using MobyLabWebProgramming.Infrastructure.Configurations;
using MobyLabWebProgramming.Infrastructure.Extensions;
using MobyLabWebProgramming.Infrastructure.Services.Implementations;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;
using MobyLabWebProgramming.Infrastructure.Workers;


var builder = WebApplication.CreateBuilder(args);

// Configure Services
builder.AddCorsConfiguration()
    .AddRepository()
    .AddAuthorizationWithSwagger("MobyLab Web App")
    .AddServices()
    .UseLogger()
    .AddWorkers();

// Add Controllers
builder.Services.AddControllers();
builder.AddApi();

// Add TMDB & Mail Config
builder.Services.Configure<TMDBConfiguration>(builder.Configuration.GetSection("TMDB"));
builder.Services.Configure<MailConfiguration>(builder.Configuration.GetSection("MailConfiguration"));

// Register Domain Services
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<ITmdbSeederService, TmdbSeederService>();
builder.Services.AddHostedService<TmdbImportWorker>();
builder.Services.AddScoped<IEventService, EventService>();

var app = builder.Build();

// Seed DB with TMDB data ONCE
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<ITmdbSeederService>();
    await seeder.SeedGenresAndMoviesAsync();
}

// Configure Middleware
app.ConfigureApplication();
app.MapControllers();

app.Run();