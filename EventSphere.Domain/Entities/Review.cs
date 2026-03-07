// Representerar en recension som en användare lämnar efter ett event.
// En användare kan bara skriva EN recension per event.
// 
// IsVerifiedAttendee sätts automatiskt till true om användaren
// faktiskt hade en bokning på eventet – ger trovärdighet åt recensionen.

using EventSphere.Domain.Common;

namespace EventSphere.Domain.Entities;

public class Review : BaseEntity
{
    // Vem som skrivit recensionen
    public Guid UserId { get; set; }

    // Vilket event recensionen gäller
    public Guid EventId { get; set; }

    // Betyg 1–5 stjärnor
    public int Rating { get; set; }

    // Recensionstext, minst 10 och max 1000 tecken
    public string Comment { get; set; } = string.Empty;

    // True om användaren faktiskt var på eventet
    public bool IsVerifiedAttendee { get; set; } = false;

    // Navigationsegenskap – laddas med Include() i repository
    public User User { get; set; } = null!;
    public Event Event { get; set; } = null!;
}