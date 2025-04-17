using System.Net;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Core.Enums;
using MobyLabWebProgramming.Core.Specifications;
using MobyLabWebProgramming.Infrastructure.Database;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;

namespace MobyLabWebProgramming.Infrastructure.Services.Implementations;

public class EventService : IEventService
{
    private readonly IRepository<WebAppDatabaseContext> _repo;

    public EventService(IRepository<WebAppDatabaseContext> repo) => _repo = repo;

    public async Task<EventDTO> CreateEventAsync(EventCreateDTO dto, Guid userId, UserRoleEnum role)
    {
        if (role == UserRoleEnum.User && dto.MaxParticipants > 10)
        {
            throw new ServerException(HttpStatusCode.BadRequest, "Userul standard poate crea evenimente cu maxim 10 participanți.");
        }

        var movie = await _repo.GetAsync<Movie>(dto.MovieId);
        if (movie is null)
            throw new ServerException(HttpStatusCode.BadRequest, "Movie does not exist in local database.");

        var newEvent = new Event
        {
            Title = dto.Title,
            Description = dto.Description,
            Location = dto.Location,
            Date = dto.Date,
            MaxParticipants = dto.MaxParticipants,
            FreeSeats = dto.MaxParticipants,
            OrganizerId = userId,
            MovieId = dto.MovieId
        };

        await _repo.AddAsync(newEvent);

        return new EventDTO
        {
            Id = newEvent.Id,
            Title = newEvent.Title,
            Description = newEvent.Description,
            Location = newEvent.Location,
            Date = newEvent.Date,
            MaxParticipants = newEvent.MaxParticipants,
            FreeSeats = newEvent.FreeSeats,
            OrganizerId = userId,
            MovieId = newEvent.MovieId,
            CreatedAt = newEvent.CreatedAt
        };
    }
    
    public async Task<EventDTO> UpdateEventAsync(Guid eventId, EventCreateDTO dto, Guid userId, UserRoleEnum role)
    {
        var ev = await _repo.GetAsync(new EventSpec(eventId, includeDetails: true));
        if (ev == null || ev.OrganizerId != userId)
            throw new ServerException(HttpStatusCode.Forbidden, "Access denied or event not found.");
        
        if (ev.UserEvents.Count == 0 && role == UserRoleEnum.User && dto.MaxParticipants > 10)
        {
            throw new ServerException(HttpStatusCode.BadRequest, "Userul standard poate seta maxim 10 participanți.");
        }

        ev.Title = dto.Title;
        ev.Description = dto.Description;
        ev.Location = dto.Location;
        ev.Date = dto.Date;

        if (ev.UserEvents.Count == 0)
        {
            ev.MaxParticipants = dto.MaxParticipants;
            ev.FreeSeats = dto.MaxParticipants;
        }

        await _repo.UpdateAsync(ev);

        return new EventDTO
        {
            Id = ev.Id,
            Title = ev.Title,
            Description = ev.Description,
            Location = ev.Location,
            Date = ev.Date,
            MaxParticipants = ev.MaxParticipants,
            FreeSeats = ev.FreeSeats,
            OrganizerId = ev.OrganizerId,
            MovieId = ev.MovieId,
            CreatedAt = ev.CreatedAt
        };
    }


    public async Task DeleteEventAsync(Guid eventId, Guid userId)
    {
        var ev = await _repo.GetAsync(new EventSpec(eventId));
        if (ev == null || ev.OrganizerId != userId)
            throw new ServerException(HttpStatusCode.Forbidden, "Access denied or event not found.");

        await _repo.DeleteAsync<Event>(eventId);
    }

    public async Task<List<EventDTO>> GetEventsByLocationAsync(string location) =>
        await _repo.ListAsync(new EventProjectionSpec(location));

    public async Task<List<EventDTO>> GetEventsByDayAsync(DateTime date) =>
        await _repo.ListAsync(new EventProjectionSpec(date.Date, date.Date.AddDays(1)));

    public async Task<List<EventDTO>> GetEventsByFullDateTimeAsync(DateTime dateTime) =>
        await _repo.ListAsync(new EventProjectionSpec(dateTime, dateTime));

    public async Task<List<EventDTO>> GetEventsByMonthAsync(int year, int month)
    {
        var start = new DateTime(year, month, 1);
        var end = start.AddMonths(1).AddTicks(-1);
        return await _repo.ListAsync(new EventProjectionSpec(start, end));
    }

    public async Task<List<EventDTO>> GetEventsByMovieIdAsync(Guid movieId) =>
        await _repo.ListAsync(new EventProjectionSpec(movieId));

    public async Task<EventDTO?> GetEventByIdAsync(Guid id) =>
        await _repo.GetAsync(new EventProjectionSpec(id));
    
    public async Task<List<EventDTO>> GetEventsByMovieTitleAsync(string title)
    {
        return await _repo.ListAsync(new EventByMovieTitleSpec(title));
    }

    public async Task<Movie?> GetMovieById(Guid movieId)
    {
        var spec = new MovieByIdSpec(movieId);
        return await _repo.GetAsync(spec);
    }


}
