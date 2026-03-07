using FluentValidation;
using EventSphere.Application.DTOs.Event;
using EventSphere.Application.DTOs.User;
using EventSphere.Application.DTOs.Venue;
using EventSphere.Application.DTOs.Registration;
using EventSphere.Application.DTOs.Review;

namespace EventSphere.Application.Validators;

public class CreateEventValidator : AbstractValidator<CreateEventDto>
{
    public CreateEventValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MinimumLength(20).WithMessage("Description must be at least 20 characters.");

        // Startdatum måste vara i framtiden
        RuleFor(x => x.StartDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Start date must be in the future.");

        // Slutdatum måste vara efter startdatum
        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.");

        // Pris kan vara 0 (gratis) men inte negativt
        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.");

        RuleFor(x => x.MaxAttendees)
            .GreaterThan(0).WithMessage("Max attendees must be greater than 0.");

        RuleFor(x => x.VenueId).NotEmpty().WithMessage("Venue is required.");
        RuleFor(x => x.CategoryId).NotEmpty().WithMessage("Category is required.");
        RuleFor(x => x.OrganizerId).NotEmpty().WithMessage("Organizer is required.");

        // OnlineUrl krävs bara om eventet är online
        When(x => x.IsOnline, () =>
        {
            RuleFor(x => x.OnlineUrl)
                .NotEmpty().WithMessage("Online URL is required for online events.")
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                .WithMessage("Online URL must be a valid URL.");
        });
    }
}

public class CreateUserValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        // Lösenord måste ha minst 8 tecken, en versal och en siffra
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(😎.WithMessage("Password must be at least 8 characters.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.");

        RuleFor(x => x.Role)
            .Must(r => new[] { "Attendee", "Organizer", "Admin" }.Contains(r))
            .WithMessage("Role must be Attendee, Organizer, or Admin.");
    }
}

public class CreateVenueValidator : AbstractValidator<CreateVenueDto>
{
    public CreateVenueValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Capacity).GreaterThan(0).WithMessage("Capacity must be greater than 0.");

        // Koordinater måste ligga inom giltiga geografiska gränser
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
    }
}

public class CreateRegistrationValidator : AbstractValidator<CreateRegistrationDto>
{
    public CreateRegistrationValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.");
        RuleFor(x => x.EventId).NotEmpty().WithMessage("EventId is required.");
    }
}

public class CreateReviewValidator : AbstractValidator<CreateReviewDto>
{
    public CreateReviewValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.EventId).NotEmpty();

        // Betyg måste vara mellan 1 och 5
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage("Comment is required.")
            .MinimumLength(10).WithMessage("Comment must be at least 10 characters.")
            .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters.");
    }
}