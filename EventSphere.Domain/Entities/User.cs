// Representerar en användare i systemet.
// En användare kan vara Attendee (besökare), 
// Organizer (arrangör) eller Admin.

using EventSphere.Domain.Common;

namespace EventSphere.Domain.Entities;

public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    // Email är unik – ingen kan registrera samma email två gånger
    public string Email { get; set; } = string.Empty;

    // Lösenordet lagras aldrig i klartext – bara som hash
    public string PasswordHash { get; set; } = string.Empty;

    // Styr vad användaren får göra i systemet
    public string Role { get; set; } = "Attendee";

    public string? ProfileImageUrl { get; set; }
    public string? Bio { get; set; }

    // En användare kan ha många events, bokningar och recensioner
    public ICollection<Event> OrganizedEvents { get; set; } = new List<Event>();
    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}