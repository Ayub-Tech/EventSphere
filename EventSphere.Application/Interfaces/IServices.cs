using EventSphere.Application.DTOs.Event;
using EventSphere.Application.DTOs.User;
using EventSphere.Application.DTOs.Venue;
using EventSphere.Application.DTOs.Registration;
using EventSphere.Application.DTOs.Review;

namespace EventSphere.Application.Interfaces;

public interface IEventService
{
    // Hämta filtrerade, sorterade och paginerade events
    Task<PagedEventResult> GetFilteredAsync(EventQueryParams query);
    Task<EventDto?> GetByIdAsync(Guid id);

    // Närmaste kommande events – för startsidan
    Task<IEnumerable<EventDto>> GetUpcomingAsync(int count = 10);
    Task<IEnumerable<EventDto>> GetByOrganizerAsync(Guid organizerId);
    Task<EventDto> CreateAsync(CreateEventDto dto);
    Task<EventDto> UpdateAsync(Guid id, UpdateEventDto dto);
    Task DeleteAsync(Guid id);

    // Ändrar status till Published
    Task<EventDto> PublishAsync(Guid id);

    // Ändrar status till Cancelled
    Task<EventDto> CancelAsync(Guid id);
}

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto> CreateAsync(CreateUserDto dto);
    Task<UserDto> UpdateAsync(Guid id, UpdateUserDto dto);
    Task DeleteAsync(Guid id);

    // Verifierar lösenord och returnerar JWT-token
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}

public interface IVenueService
{
    Task<IEnumerable<VenueDto>> GetAllAsync();
    Task<VenueDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<VenueDto>> SearchByLocationAsync(string city, string? country = null);
    Task<VenueDto> CreateAsync(CreateVenueDto dto);
    Task<VenueDto> UpdateAsync(Guid id, UpdateVenueDto dto);
    Task DeleteAsync(Guid id);
}

public interface IRegistrationService
{
    Task<IEnumerable<RegistrationDto>> GetByEventAsync(Guid eventId);
    Task<IEnumerable<RegistrationDto>> GetByUserAsync(Guid userId);

    // Kontrollerar kapacitet och dubbelbokning innan bokning skapas
    Task<RegistrationDto> RegisterAsync(CreateRegistrationDto dto);
    Task<RegistrationDto> UpdateStatusAsync(Guid id, UpdateRegistrationStatusDto dto);

    // Markerar deltagaren som incheckad
    Task<RegistrationDto> CheckInAsync(Guid id);
    Task CancelAsync(Guid id);
}

public interface IReviewService
{
    Task<IEnumerable<ReviewDto>> GetByEventAsync(Guid eventId);
    Task<ReviewDto> CreateAsync(CreateReviewDto dto);
    Task DeleteAsync(Guid id);
}