using Microsoft.AspNetCore.Mvc;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;

namespace MobyLabWebProgramming.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MovieController(IMovieService movieService) : ControllerBase
{
    [HttpGet("all")]
    public async Task<ActionResult<List<MovieDetailsDTO>>> GetAllMovies()
    {
        var result = await movieService.GetAllMoviesAsync();
        return Ok(result);
    }

    [HttpGet("by-title")]
    public async Task<ActionResult<List<MovieDetailsDTO>>> GetByTitle([FromQuery] string title)
    {
        var result = await movieService.GetMoviesByTitleAsync(title);
        return Ok(result);
    }

    [HttpGet("by-genre")]
    public async Task<ActionResult<List<MovieDetailsDTO>>> GetByGenre([FromQuery] string genre)
    {
        var result = await movieService.GetMoviesByGenreAsync(genre);
        return Ok(result);
    }
}