using EventSphere.Domain.Entities;
using EventSphere.Domain.Interfaces;
using EventSphere.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventSphere.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    // Sök användare via email – används vid inloggning
    public async Task<User?> GetByEmailAsync(string email)
        => await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

    // Kontrollera om email redan finns – används vid registrering
    public async Task<bool> EmailExistsAsync(string email)
        => await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
}

public class VenueRepository : Repository<Venue>, IVenueRepository
{
    public VenueRepository(AppDbContext context) : base(context) { }

    // Sök venues på stad – valfritt med land för att minska träffar
    public async Task<IEnumerable<Venue>> SearchByLocationAsync(string city, string? country = null)
    {
        var query = _context.Venues.Where(v => v.City.ToLower().Contains(city.ToLower()));
        if (!string.IsNullOrWhiteSpace(country))
            query = query.Where(v => v.Country.ToLower() == country.ToLower());
        return await query.ToListAsync();
    }
}

public class RegistrationRepository : Repository<Registration>, IRegistrationRepository
{
    public RegistrationRepository(AppDbContext context) : base(context) { }

    // Alla deltagare för ett event – inkluderar user och event för DTO
    public async Task<IEnumerable<Registration>> GetByEventAsync(Guid eventId)
        => await _context.Registrations
            .Include(r => r.User)
            .Include(r => r.Event)
            .Where(r => r.EventId == eventId)
            .ToListAsync();

    // En användares alla bokningar – senast bokade visas först
    public async Task<IEnumerable<Registration>> GetByUserAsync(Guid userId)
        => await _context.Registrations
            .Include(r => r.User)
            .Include(r => r.Event)
                .ThenInclude(e => e.Venue)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.RegisteredAt)
            .ToListAsync();

    public async Task<Registration?> GetByUserAndEventAsync(Guid userId, Guid eventId)
        => await _context.Registrations
            .FirstOrDefaultAsync(r => r.UserId == userId && r.EventId == eventId);

    // Räkna aktiva bokningar – cancelled räknas inte
    public async Task<int> GetRegistrationCountAsync(Guid eventId)
        => await _context.Registrations
            .CountAsync(r => r.EventId == eventId && r.Status != "Cancelled");

    // Kontrollera om användaren redan har en aktiv bokning
    public async Task<bool> IsUserRegisteredAsync(Guid userId, Guid eventId)
        => await _context.Registrations
            .AnyAsync(r => r.UserId == userId && r.EventId == eventId && r.Status != "Cancelled");
}

public class ReviewRepository : Repository<Review>, IReviewRepository
{
    public ReviewRepository(AppDbContext context) : base(context) { }

    // Alla recensioner för ett event – nyast först
    public async Task<IEnumerable<Review>> GetByEventAsync(Guid eventId)
        => await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Event)
            .Where(r => r.EventId == eventId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    // Beräkna snittbetyg – returnerar 0 om inga recensioner finns
    public async Task<double> GetAverageRatingAsync(Guid eventId)
    {
        var reviews = await _context.Reviews
            .Where(r => r.EventId == eventId)
            .ToListAsync();
        return reviews.Any() ? reviews.Average(r => r.Rating) : 0;
    }

    // Kontrollera om användaren redan recenserat eventet
    public async Task<bool> HasUserReviewedAsync(Guid userId, Guid eventId)
        => await _context.Reviews.AnyAsync(r => r.UserId == userId && r.EventId == eventId);
}

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context) { }

    public async Task<Category?> GetByNameAsync(string name)
        => await _context.Categories.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
}