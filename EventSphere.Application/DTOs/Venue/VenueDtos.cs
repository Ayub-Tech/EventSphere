namespace EventSphere.Application.DTOs.Venue;

// Vad API:et returnerar för en venue
public class VenueDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    // Antal kommande events på denna venue
    public int UpcomingEventCount { get; set; }
}

// Vad klienten skickar för att skapa en venue
public class CreateVenueDto
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

// Uppdatering – alla fält är valfria
public class UpdateVenueDto
{
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public int? Capacity { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
}