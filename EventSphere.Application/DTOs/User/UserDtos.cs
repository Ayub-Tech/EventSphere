namespace EventSphere.Application.DTOs.User;

// Vad API:et returnerar – innehåller ALDRIG lösenord!
public class UserDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Vad klienten skickar vid registrering
public class CreateUserDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Skickas i klartext men hashas direkt i UserService
    public string Password { get; set; } = string.Empty;

    public string Role { get; set; } = "Attendee";
    public string? Bio { get; set; }
}

// Vad klienten skickar för att uppdatera sin profil
public class UpdateUserDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? Bio { get; set; }
}

// Inloggningsuppgifter
public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

// Svar vid lyckad inloggning – innehåller JWT-token
public class AuthResponseDto
{
    // Skickas med i varje request: "Authorization: Bearer {Token}"
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}