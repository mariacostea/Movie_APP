﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Enums;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;
using System.Security.Claims;

namespace MobyLabWebProgramming.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventController(IEventService eventService) : ControllerBase
{
    private Guid GetLoggedInUserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    private UserRoleEnum GetLoggedInUserRole() =>
        Enum.Parse<UserRoleEnum>(User.FindFirst(ClaimTypes.Role)!.Value);

    [HttpPost("create")]
    [Authorize]
    public async Task<ActionResult<ServiceResponse<EventDTO>>> Create([FromBody] EventCreateDTO dto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var roleString = User.FindFirst(ClaimTypes.Role)?.Value ?? "User";
        var role = Enum.Parse<UserRoleEnum>(roleString, ignoreCase: true);

        var result = await eventService.CreateEventAsync(dto, userId, role);
        return Ok(ServiceResponse.ForSuccess(result));
    }


    [HttpPut("update/{eventId}")]
    [Authorize]
    public async Task<ActionResult<ServiceResponse<EventDTO>>> Update(Guid eventId, [FromBody] EventCreateDTO dto)
    {
        var userId = GetLoggedInUserId();
        var role = GetLoggedInUserRole();

        var result = await eventService.UpdateEventAsync(eventId, dto, userId, role);
        return Ok(ServiceResponse.ForSuccess(result));
    }

    [HttpDelete("delete/{eventId}")]
    [Authorize]
    public async Task<ActionResult<ServiceResponse>> Delete(Guid eventId)
    {
        var userId = GetLoggedInUserId();
        await eventService.DeleteEventAsync(eventId, userId);
        return Ok(ServiceResponse.ForSuccess());
    }

    [HttpGet("by-location")]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceResponse<List<EventDTO>>>> ByLocation([FromQuery] string location)
    {
        var result = await eventService.GetEventsByLocationAsync(location);
        return Ok(ServiceResponse.ForSuccess(result));
    }

    [HttpGet("by-day")]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceResponse<List<EventDTO>>>> ByDay([FromQuery] DateTime date)
    {
        var result = await eventService.GetEventsByDayAsync(date);
        return Ok(ServiceResponse.ForSuccess(result));
    }

    [HttpGet("by-full-date")]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceResponse<List<EventDTO>>>> ByFullDate([FromQuery] DateTime dateTime)
    {
        var result = await eventService.GetEventsByFullDateTimeAsync(dateTime);
        return Ok(ServiceResponse.ForSuccess(result));
    }

    [HttpGet("by-month")]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceResponse<List<EventDTO>>>> ByMonth([FromQuery] int year, [FromQuery] int month)
    {
        var result = await eventService.GetEventsByMonthAsync(year, month);
        return Ok(ServiceResponse.ForSuccess(result));
    }

    [HttpGet("by-movie-title")]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceResponse<List<EventDTO>>>> ByMovieTitle([FromQuery] string title)
    {
        var result = await eventService.GetEventsByMovieTitleAsync(title);
        return Ok(ServiceResponse.ForSuccess(result));
    }


    [HttpGet("by-id")]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceResponse<EventDTO?>>> ById([FromQuery] Guid id)
    {
        var result = await eventService.GetEventByIdAsync(id);
        return Ok(ServiceResponse.ForSuccess(result));
    }
}