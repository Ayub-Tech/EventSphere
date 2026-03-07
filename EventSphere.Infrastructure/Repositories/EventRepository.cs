using EventSphere.Domain.Entities;
using EventSphere.Domain.Interfaces;
using EventSphere.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventSphere.Infrastructure.Repositories;

public class EventRepository : Repository<Event>, IEventRepository
{
    public EventRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Event>> GetFilteredAsync(
        string? search, Guid? categoryId, string? city,
        DateTime? fromDate, DateTime? toDate, decimal? maxPrice,
        string? status, string? sortBy, bool ascending = true,
        int page = 1, int pageSize = 10)
    {
        // Starta query med relaterad data inladdad
        var query = _context.Events
            .Include(e => e.Organizer)
            .Include(e => e.Venue)
            .Include(e => e.Category)
            .AsQueryable();

        // Applicera filter – bara om parametern är satt
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(e =>
                e.Title.Contains(search) ||
                e.Description.Contains(search) ||
                (e.Tags != null && e.Tags.Contains(search)));

        if (categoryId.HasValue)
            query = query.Where(e => e.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(e => e.Venue.City.ToLower() == city.ToLower());

        if (fromDate.HasValue)
            query = query.Where(e => e.StartDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(e => e.StartDate <= toDate.Value);

        if (maxPrice.HasValue)
            query = query.Where(e => e.Price <= maxPrice.Value);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(e => e.Status == status);

        // Sortera baserat på valt fält
        query = sortBy?.ToLower() switch
        {
            "title" => ascending ? query.OrderBy(e => e.Title) : query.OrderByDescending(e => e.Title),
            "price" => ascending ? query.OrderBy(e => e.Price) : query.OrderByDescending(e => e.Price),
            "createdat" => ascending ? query.OrderBy(e => e.CreatedAt) : query.OrderByDescending(e => e.CreatedAt),
            _ => ascending ? query.OrderBy(e => e.StartDate) : query.OrderByDescending(e => e.StartDate)
        };

        // Paginering – hoppa över föregående sidor och ta max pageSize poster
        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    // Räkna totalt antal matchande events för paginering
    public async Task<int> GetTotalCountAsync(string? search, Guid? categoryId, string? city, string? status)
    {
        var query = _context.Events.Include(e => e.Venue).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(e => e.Title.Contains(search) || e.Description.Contains(search));
        if (categoryId.HasValue)
            query = query.Where(e => e.CategoryId == categoryId.Value);
        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(e => e.Venue.City.ToLower() == city.ToLower());
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(e => e.Status == status);

        return await query.CountAsync();
    }

    // Alla events skapade av en organisatör – nyast först
    public async Task<IEnumerable<Event>> GetByOrganizerAsync(Guid organizerId)
        => await _context.Events
            .Include(e => e.Venue)
            .Include(e => e.Category)
            .Where(e => e.OrganizerId == organizerId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();

    // Hämta ett event med all relaterad data
    public async Task<Event?> GetWithDetailsAsync(Guid id)
        => await _context.Events
            .Include(e => e.Organizer)
            .Include(e => e.Venue)
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == id);

    // Närmaste kommande publiserade events
    public async Task<IEnumerable<Event>> GetUpcomingAsync(int count = 10)
        => await _context.Events
            .Include(e => e.Organizer)
            .Include(e => e.Venue)
            .Include(e => e.Category)
            .Where(e => e.Status == "Published" && e.StartDate > DateTime.UtcNow)
            .OrderBy(e => e.StartDate)
            .Take(count)
            .ToListAsync();
}