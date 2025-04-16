using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Infrastructure.Authorization;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;
using System.Security.Claims;

namespace MobyLabWebProgramming.Backend.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Add([FromBody] ReviewDTO dto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _reviewService.AddReviewAsync(dto, userId);
        return Ok(new { message = "Review added successfully." });
    }
    
    [HttpPut("{reviewId}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid reviewId, [FromBody] ReviewDTO dto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _reviewService.UpdateReviewAsync(reviewId, dto, userId);
        return Ok(new { message = "Review updated successfully." });
    }

    [HttpDelete("{reviewId}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid reviewId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _reviewService.DeleteReviewAsync(reviewId, userId);
        return Ok(new { message = "Review deleted successfully." });
    }

}
