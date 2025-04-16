using Microsoft.AspNetCore.Mvc;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;

namespace MobyLabWebProgramming.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MovieController(IMovieService movieService) : ControllerBase
{
    [HttpGet("all")]
    public async Task<ActionResult<PagedResponse<MovieDetailsDTO>>> GetAll([FromQuery] PageDTO request)
    {
        var response = await movieService.GetAllMoviesAsync(request.Page, request.PageSize);
        return Ok(response);
    }

    [HttpGet("by-title")]
    public async Task<IActionResult> GetExactMovie([FromQuery] string title)
    {
        var movie = await movieService.GetMovieByTitleAsync(title);
        return movie is not null ? Ok(movie) : NotFound();
    }
    
    [HttpGet("by-genre")]
    public async Task<ActionResult<PagedResponse<MovieDetailsDTO>>> GetByGenre([FromQuery] string genre, [FromQuery] PageDTO request)
    {
        var response = await movieService.GetMoviesByGenreAsync(genre, request.Page, request.PageSize);
        return Ok(response);
    }
}