using EventSphere.Application.DTOs.Event;
using EventSphere.Application.DTOs.User;
using EventSphere.Application.DTOs.Venue;
using EventSphere.Application.DTOs.Registration;
using EventSphere.Application.DTOs.Review;
using EventSphere.Domain.Entities;

namespace EventSphere.Application.Mappings;

public static class MappingExtensions
{
    // Konverterar User-entitet till UserDto – lösenord inkluderas ALDRIG
    public static UserDto ToDto(this User user) => new()
    {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        Role = user.Role,
        ProfileImageUrl = user.ProfileImageUrl,
        Bio = user.Bio,
        CreatedAt = user.CreatedAt
    };

    // Skapar User-entitet från DTO – lösenord hashas separat i UserService
    public static User ToEntity(this CreateUserDto dto) => new()
    {
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        Email = dto.Email,
        Role = dto.Role,
        Bio = dto.Bio
    };

    // Konverterar Venue-entitet till VenueDto
    public static VenueDto ToDto(this Venue venue, int upcomingEventCount = 0) => new()
    {
        Id = venue.Id,
        Name = venue.Name,
        Address = venue.Address,
        City = venue.City,
        Country = venue.Country,
        Capacity = venue.Capacity,
        Description = venue.Description,
        ImageUrl = venue.ImageUrl,
        Latitude = venue.Latitude,
        Longitude = venue.Longitude,
        UpcomingEventCount = upcomingEventCount
    };

    // Skapar Venue-entitet från DTO
    public static Venue ToEntity(this CreateVenueDto dto) => new()
    {
        Name = dto.Name,
        Address = dto.Address,
        City = dto.City,
        Country = dto.Country,
        Capacity = dto.Capacity,
        Description = dto.Description,
        ImageUrl = dto.ImageUrl,
        Latitude = dto.Latitude,
        Longitude = dto.Longitude
    };

    // Konverterar Event-entitet till EventDto
    // currentAttendees och avgRating beräknas separat av EventService
    public static EventDto ToDto(this Event ev, int currentAttendees = 0, double avgRating = 0, int reviewCount = 0) => new()
    {
        Id = ev.Id,
        Title = ev.Title,
        Description = ev.Description,
        StartDate = ev.StartDate,
        EndDate = ev.EndDate,
        Price = ev.Price,
        MaxAttendees = ev.MaxAttendees,
        CurrentAttendees = currentAttendees,
        Status = ev.Status,
        ImageUrl = ev.ImageUrl,
        IsOnline = ev.IsOnline,
        OnlineUrl = ev.OnlineUrl,
        Tags = ev.Tags,
        CreatedAt = ev.CreatedAt,
        OrganizerId = ev.OrganizerId,
        OrganizerName = ev.Organizer != null ? $"{ev.Organizer.FirstName} {ev.Organizer.LastName}" : string.Empty,
        CategoryId = ev.CategoryId,
        CategoryName = ev.Category?.Name ?? string.Empty,
        VenueId = ev.VenueId,
        VenueName = ev.Venue?.Name ?? string.Empty,
        VenueCity = ev.Venue?.City ?? string.Empty,
        AverageRating = avgRating,
        ReviewCount = reviewCount
    };

    // Skapar Event-entitet – status sätts alltid till Draft vid skapande
    public static Event ToEntity(this CreateEventDto dto) => new()
    {
        Title = dto.Title,
        Description = dto.Description,
        StartDate = dto.StartDate,
        EndDate = dto.EndDate,
        Price = dto.Price,
        MaxAttendees = dto.MaxAttendees,
        ImageUrl = dto.ImageUrl,
        IsOnline = dto.IsOnline,
        OnlineUrl = dto.OnlineUrl,
        Tags = dto.Tags,
        VenueId = dto.VenueId,
        CategoryId = dto.CategoryId,
        OrganizerId = dto.OrganizerId,
        Status = "Draft"
    };

    // Konverterar Registration-entitet till RegistrationDto
    public static RegistrationDto ToDto(this Registration reg) => new()
    {
        Id = reg.Id,
        UserId = reg.UserId,
        UserName = reg.User != null ? $"{reg.User.FirstName} {reg.User.LastName}" : string.Empty,
        UserEmail = reg.User?.Email ?? string.Empty,
        EventId = reg.EventId,
        EventTitle = reg.Event?.Title ?? string.Empty,
        Status = reg.Status,
        RegisteredAt = reg.RegisteredAt,
        TicketCode = reg.TicketCode,
        CheckedIn = reg.CheckedIn,
        CheckedInAt = reg.CheckedInAt
    };

    // Konverterar Review-entitet till ReviewDto
    public static ReviewDto ToDto(this Review review) => new()
    {
        Id = review.Id,
        UserId = review.UserId,
        UserName = review.User != null ? $"{review.User.FirstName} {review.User.LastName}" : string.Empty,
        UserProfileImage = review.User?.ProfileImageUrl,
        EventId = review.EventId,
        EventTitle = review.Event?.Title ?? string.Empty,
        Rating = review.Rating,
        Comment = review.Comment,
        IsVerifiedAttendee = review.IsVerifiedAttendee,
        CreatedAt = review.CreatedAt
    };
}