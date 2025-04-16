namespace MobyLabWebProgramming.Backend.Controllers;
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;
using MobyLabWebProgramming.Infrastructure.Database;


[ApiController]
[Route("api/[controller]/[action]")]
public class UserMovieController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly IRepository<WebAppDatabaseContext>
        _userMovieRepo;

    public UserMovieController(IMovieService movieService, IRepository<WebAppDatabaseContext>
        userMovieRepo)
    {
        _movieService = movieService;
        _userMovieRepo = userMovieRepo;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> MarkAsWatched([FromBody] MovieDTO movieDto)
    {
        var movie = await _movieService.AddOrGetMovieFromApi(movieDto);
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var userMovie = new UserMovie
        {
            UserId = userId,
            MovieId = movie.Id,
            IsWatched = true,
            WatchedOn = DateTime.UtcNow
        };

        await _userMovieRepo.AddAsync(userMovie);

        return Ok(new { message = "Movie added to watched list", movieId = movie.Id });
    }
}


