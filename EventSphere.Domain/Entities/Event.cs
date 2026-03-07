// Huvudentiteten i systemet – representerar ett evenemang.
// Ett event kopplar ihop en Organizer, en Venue och en Category.
// 
// Status-flödet:
// Draft → Published → Completed
//              ↓
//          Cancelled

using EventSphere.Domain.Common;

namespace EventSphere.Domain.Entities;

public class Event : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Start och sluttid för eventet
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    // Biljettpris – 0 betyder gratis
    public decimal Price { get; set; }

    // Max antal bokningar som tillåts
    public int MaxAttendees { get; set; }

    // Draft = ej synligt, Published = öppet för bokning
    public string Status { get; set; } = "Draft";

    public string? ImageUrl { get; set; }

    // Om true måste OnlineUrl anges
    public bool IsOnline { get; set; } = false;
    public string? OnlineUrl { get; set; }

    // Komma-separerade taggar, t.ex. "musik,utomhus,familj"
    public string? Tags { get; set; }

    // Kopplingar till andra entiteter
    public Guid OrganizerId { get; set; }
    public Guid VenueId { get; set; }
    public Guid CategoryId { get; set; }

    // Navigationsegenskap – laddas med Include() i repository
    public User Organizer { get; set; } = null!;
    public Venue Venue { get; set; } = null!;
    public Category Category { get; set; } = null!;
    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}