using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class EventProjectionSpec : Specification<Event, EventDTO>
{
    public EventProjectionSpec(bool orderByCreatedAt = false)
    {
        Query.Select(e => new EventDTO
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            Location = e.Location,
            Date = e.Date,
            MaxParticipants = e.MaxParticipants,
            FreeSeats = e.FreeSeats,
            OrganizerId = e.OrganizerId,
            CreatedAt = e.CreatedAt
        });

        if (orderByCreatedAt)
        {
            Query.OrderByDescending(e => e.CreatedAt);
        }
    }
    
    public EventProjectionSpec(Guid id) : this()
    {
        Query.Where(e => e.Id == id);
    }
    
    public EventProjectionSpec(string? search) : this(true)
    {
        if (string.IsNullOrWhiteSpace(search)) return;

        var keyword = $"%{search.Trim().Replace(" ", "%")}%";

        Query.Where(e =>
            EF.Functions.ILike(e.Title, keyword) ||
            EF.Functions.ILike(e.Description ?? "", keyword) ||
            EF.Functions.ILike(e.Location, keyword));
    }
    
    public EventProjectionSpec(DateTime date) : this()
    {
        Query.Where(e => e.Date.Date == date.Date);
    }
    
    public EventProjectionSpec(Guid movieId, bool byMovie = true) : this()
    {
        if (byMovie)
        {
            Query.Where(e => e.MovieId == movieId);
        }
    }
    
    public EventProjectionSpec(DateTime from, DateTime to) : this()
    {
        Query.Where(e => e.Date >= from && e.Date <= to);
    }

}