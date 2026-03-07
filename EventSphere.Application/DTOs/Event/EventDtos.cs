namespace EventSphere.Application.DTOs.Event;

// Vad API:et returnerar när någon hämtar ett event
public class EventDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Price { get; set; }
    public int MaxAttendees { get; set; }

    // Beräknas dynamiskt – hur många har bokat?
    public int CurrentAttendees { get; set; }

    public string Status { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool IsOnline { get; set; }
    public string? OnlineUrl { get; set; }
    public string? Tags { get; set; }
    public DateTime CreatedAt { get; set; }

    // Info om organisatören
    public string OrganizerName { get; set; } = string.Empty;
    public Guid OrganizerId { get; set; }

    // Info om kategorin
    public string CategoryName { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }

    // Info om lokalen
    public string VenueName { get; set; } = string.Empty;
    public string VenueCity { get; set; } = string.Empty;
    public Guid VenueId { get; set; }

    // Beräknas dynamiskt från recensioner
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
}

// Vad klienten skickar när ett nytt event skapas
public class CreateEventDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Price { get; set; }
    public int MaxAttendees { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsOnline { get; set; }
    public string? OnlineUrl { get; set; }
    public string? Tags { get; set; }
    public Guid VenueId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid OrganizerId { get; set; }
}

// Uppdatering – alla fält är nullable, bara det som skickas ändras
public class UpdateEventDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? Price { get; set; }
    public int? MaxAttendees { get; set; }
    public string? ImageUrl { get; set; }
    public bool? IsOnline { get; set; }
    public string? OnlineUrl { get; set; }
    public string? Tags { get; set; }
    public string? Status { get; set; }
    public Guid? VenueId { get; set; }
    public Guid? CategoryId { get; set; }
}

// Query-parametrar för GET /api/events – alla är valfria
public class EventQueryParams
{
    public string? Search { get; set; }
    public Guid? CategoryId { get; set; }
    public string? City { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Status { get; set; }
    public string SortBy { get; set; } = "StartDate";
    public bool Ascending { get; set; } = true;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

// Paginerat svar – innehåller events + metadata om sidnummer
public class PagedEventResult
{
    public IEnumerable<EventDto> Items { get; set; } = new List<EventDto>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }

    // Beräknas automatiskt
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}