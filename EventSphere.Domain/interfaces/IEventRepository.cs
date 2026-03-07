using EventSphere.Domain.Entities;

namespace EventSphere.Domain.Interfaces;

public interface IEventRepository : IRepository<Event>
{
    // Avancerad filtrering, sortering och paginering
    Task<IEnumerable<Event>> GetFilteredAsync(
        string? search,
        Guid? categoryId,
        string? city,
        DateTime? fromDate,
        DateTime? toDate,
        decimal? maxPrice,
        string? status,
        string? sortBy,
        bool ascending = true,
        int page = 1,
        int pageSize = 10);

    // Totalt antal matchande events (för paginering)
    Task<int> GetTotalCountAsync(string? search, Guid? categoryId, string? city, string? status);

    // Alla events skapade av en specifik organisatör
    Task<IEnumerable<Event>> GetByOrganizerAsync(Guid organizerId);

    // Hämta ett event med all relaterad data (Venue, Category, Organizer)
    Task<Event?> GetWithDetailsAsync(Guid id);

    // Närmaste kommande publiserade events
    Task<IEnumerable<Event>> GetUpcomingAsync(int count = 10);
}