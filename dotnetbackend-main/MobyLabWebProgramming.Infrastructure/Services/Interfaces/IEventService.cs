using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Enums;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;

public interface IEventService
{
    Task<EventDTO> CreateEventAsync(EventCreateDTO dto, Guid userId, UserRoleEnum role);
    Task<EventDTO> UpdateEventAsync(Guid eventId, EventCreateDTO dto, Guid userId, UserRoleEnum role);
    Task DeleteEventAsync(Guid eventId, Guid userId);
    Task<List<EventDTO>> GetEventsByLocationAsync(string location);
    Task<List<EventDTO>> GetEventsByDayAsync(DateTime date);
    Task<List<EventDTO>> GetEventsByFullDateTimeAsync(DateTime dateTime);
    Task<List<EventDTO>> GetEventsByMonthAsync(int year, int month);
    Task<List<EventDTO>> GetEventsByMovieTitleAsync(string title);

    Task<EventDTO?> GetEventByIdAsync(Guid id);
}
