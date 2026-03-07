namespace EventSphere.Application.DTOs.Registration;

// Vad API:et returnerar för en bokning
public class RegistrationDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public Guid EventId { get; set; }
    public string EventTitle { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; }

    // Unik biljettkod, t.ex. "EVT-A3F9B21C"
    public string? TicketCode { get; set; }
    public bool CheckedIn { get; set; }
    public DateTime? CheckedInAt { get; set; }
}

// Vad klienten skickar för att boka ett event
public class CreateRegistrationDto
{
    public Guid UserId { get; set; }
    public Guid EventId { get; set; }
}

// För att ändra bokningsstatus manuellt
public class UpdateRegistrationStatusDto
{
    public string Status { get; set; } = string.Empty;
}

namespace EventSphere.Application.DTOs.Review;

// Vad API:et returnerar för en recension
public class ReviewDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? UserProfileImage { get; set; }
    public Guid EventId { get; set; }
    public string EventTitle { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;

    // True om recensenten faktiskt var på eventet
    public bool IsVerifiedAttendee { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Vad klienten skickar för att skapa en recension
public class CreateReviewDto
{
    public Guid UserId { get; set; }
    public Guid EventId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}

// Vad API:et returnerar för en kategori
public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public int EventCount { get; set; }
}

// Vad klienten skickar för att skapa en kategori
public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
}