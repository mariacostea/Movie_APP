using Microsoft.AspNetCore.Mvc;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;
using System.Net;

namespace MobyLabWebProgramming.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MovieController(IMovieService movieService) : ControllerBase
{
    [HttpGet("all")]
    public async Task<ActionResult> GetAll([FromQuery] PageDTO request)
    {
        try
        {
            var response = await movieService.GetAllMoviesAsync(request.Page, request.PageSize);
            return Ok(ServiceResponse.ForSuccess(response));
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError,
                ServiceResponse.FromError<PagedResponse<MovieDetailsDTO>>(new ErrorMessage(
                    HttpStatusCode.InternalServerError,
                    $"Eroare la preluarea filmelor: {ex.Message}",
                    ErrorCodes.TechnicalError)));
        }
    }

    [HttpGet("by-title")]
    public async Task<ActionResult> GetExactMovie([FromQuery] string title, [FromQuery] int year)
    {
        try
        {
            var movie = await movieService.GetMovieByTitleAsync(title, year);

            return movie is not null
                ? Ok(ServiceResponse.ForSuccess(movie))
                : NotFound(ServiceResponse.FromError<MovieDetailsDTO>(new ErrorMessage(
                    HttpStatusCode.NotFound,
                    "Filmul nu a fost găsit.",
                    ErrorCodes.EntityNotFound)));
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError,
                ServiceResponse.FromError<MovieDetailsDTO>(new ErrorMessage(
                    HttpStatusCode.InternalServerError,
                    $"Eroare la căutarea filmului: {ex.Message}",
                    ErrorCodes.TechnicalError)));
        }
    }


    [HttpGet("by-genre")]
    public async Task<ActionResult> GetByGenre([FromQuery] string genre, [FromQuery] PageDTO request)
    {
        try
        {
            var response = await movieService.GetMoviesByGenreAsync(genre, request.Page, request.PageSize);
            return Ok(ServiceResponse.ForSuccess(response));
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError,
                ServiceResponse.FromError<PagedResponse<MovieDetailsDTO>>(new ErrorMessage(
                    HttpStatusCode.InternalServerError,
                    $"Eroare la filtrarea după gen: {ex.Message}",
                    ErrorCodes.TechnicalError)));
        }
    }
}
