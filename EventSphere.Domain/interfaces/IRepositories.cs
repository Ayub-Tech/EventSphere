using EventSphere.Domain.Entities;

namespace EventSphere.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    // Hämta användare via email – används vid inloggning
    Task<User?> GetByEmailAsync(string email);

    // Kontrollera om email redan är registrerad
    Task<bool> EmailExistsAsync(string email);
}

public interface IVenueRepository : IRepository<Venue>
{
    // Sök venues på stad och land
    Task<IEnumerable<Venue>> SearchByLocationAsync(string city, string? country = null);
}

public interface IRegistrationRepository : IRepository<Registration>
{
    // Alla bokningar för ett event
    Task<IEnumerable<Registration>> GetByEventAsync(Guid eventId);

    // Alla bokningar gjorda av en användare
    Task<IEnumerable<Registration>> GetByUserAsync(Guid userId);

    // Hämta specifik bokning via user + event
    Task<Registration?> GetByUserAndEventAsync(Guid userId, Guid eventId);

    // Antal aktiva bokningar – används för kapacitetskontroll
    Task<int> GetRegistrationCountAsync(Guid eventId);

    // Kontrollera om användaren redan är bokad
    Task<bool> IsUserRegisteredAsync(Guid userId, Guid eventId);
}

public interface IReviewRepository : IRepository<Review>
{
    // Alla recensioner för ett event
    Task<IEnumerable<Review>> GetByEventAsync(Guid eventId);

    // Snittbetyg för ett event, t.ex. 4.3 av 5
    Task<double> GetAverageRatingAsync(Guid eventId);

    // Kontrollera om användaren redan recenserat eventet
    Task<bool> HasUserReviewedAsync(Guid userId, Guid eventId);
}

public interface ICategoryRepository : IRepository<Category>
{
    // Hämta kategori via namn
    Task<Category?> GetByNameAsync(string name);
}