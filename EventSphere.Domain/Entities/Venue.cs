// Representerar en fysisk plats där events hålls.
// T.ex. en konsertlokal eller konferenssal.
// Samma venue kan användas av flera olika events.

using EventSphere.Domain.Common;

namespace EventSphere.Domain.Entities;

public class Venue : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    // City används för platssökning – GET /api/venues/search?city=Stockholm
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    // Max antal personer som ryms – kontrolleras vid bokning
    public int Capacity { get; set; }

    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    // GPS-koordinater för kartvisning i frontend
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    // En venue kan hålla många events
    public ICollection<Event> Events { get; set; } = new List<Event>();
}