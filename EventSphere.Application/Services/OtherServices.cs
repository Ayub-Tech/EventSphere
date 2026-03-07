using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EventSphere.Application.DTOs.Registration;
using EventSphere.Application.DTOs.Review;
using EventSphere.Application.DTOs.User;
using EventSphere.Application.DTOs.Venue;
using EventSphere.Application.Interfaces;
using EventSphere.Application.Mappings;
using EventSphere.Domain.Entities;
using EventSphere.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace EventSphere.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, IConfiguration configuration, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user?.ToDto();
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(u => u.ToDto());
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        // Kontrollera att email inte redan används
        if (await _userRepository.EmailExistsAsync(dto.Email))
            throw new InvalidOperationException($"Email '{dto.Email}' is already registered.");

        var user = dto.ToEntity();

        // Hasha lösenordet med BCrypt innan det sparas
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var created = await _userRepository.AddAsync(user);
        _logger.LogInformation("Created user {Email}", dto.Email);
        return created.ToDto();
    }

    public async Task<UserDto> UpdateAsync(Guid id, UpdateUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"User {id} not found.");

        if (dto.FirstName != null) user.FirstName = dto.FirstName;
        if (dto.LastName != null) user.LastName = dto.LastName;
        if (dto.ProfileImageUrl != null) user.ProfileImageUrl = dto.ProfileImageUrl;
        if (dto.Bio != null) user.Bio = dto.Bio;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        return user.ToDto();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _userRepository.DeleteAsync(id);
        _logger.LogInformation("Deleted user {Id}", id);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        // Jämför klartextlösenord med BCrypt-hashet
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        var token = GenerateJwtToken(user);
        _logger.LogInformation("User {Email} logged in", dto.Email);
        return new AuthResponseDto { Token = token, User = user.ToDto(), ExpiresAt = DateTime.UtcNow.AddHours(24) };
    }

    // Skapar en JWT-token med användarens id, email och roll
    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class VenueService : IVenueService
{
    private readonly IVenueRepository _venueRepository;
    private readonly ILogger<VenueService> _logger;

    public VenueService(IVenueRepository venueRepository, ILogger<VenueService> logger)
    {
        _venueRepository = venueRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<VenueDto>> GetAllAsync()
        => (await _venueRepository.GetAllAsync()).Select(v => v.ToDto());

    public async Task<VenueDto?> GetByIdAsync(Guid id)
        => (await _venueRepository.GetByIdAsync(id))?.ToDto();

    public async Task<IEnumerable<VenueDto>> SearchByLocationAsync(string city, string? country = null)
        => (await _venueRepository.SearchByLocationAsync(city, country)).Select(v => v.ToDto());

    public async Task<VenueDto> CreateAsync(CreateVenueDto dto)
    {
        var created = await _venueRepository.AddAsync(dto.ToEntity());
        _logger.LogInformation("Created venue {Name}", dto.Name);
        return created.ToDto();
    }

    public async Task<VenueDto> UpdateAsync(Guid id, UpdateVenueDto dto)
    {
        var venue = await _venueRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Venue {id} not found.");

        if (dto.Name != null) venue.Name = dto.Name;
        if (dto.Address != null) venue.Address = dto.Address;
        if (dto.City != null) venue.City = dto.City;
        if (dto.Country != null) venue.Country = dto.Country;
        if (dto