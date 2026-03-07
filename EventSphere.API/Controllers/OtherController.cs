using EventSphere.Application.DTOs.Registration;
using EventSphere.Application.DTOs.Review;
using EventSphere.Application.DTOs.User;
using EventSphere.Application.DTOs.Venue;
using EventSphere.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace EventSphere.API.Controllers;

// ─── UsersController ──────────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IValidator<CreateUserDto> _validator;

    public UsersController(IUserService userService, IValidator<CreateUserDto> validator)
    {
        _userService = userService;
        _validator = validator;
    }

    // POST /api/users/register – skapa nytt konto
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] CreateUserDto dto)
    {
        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var created = await _userService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // POST /api/users/login – logga in och få JWT-token
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        => Ok(await _userService.LoginAsync(dto));

    // GET /api/users/{id} – hämta en användares profil
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    // GET /api/users – hämta alla användare
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        => Ok(await _userService.GetAllAsync());

    // PUT /api/users/{id} – uppdatera profil
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserDto>> Update(Guid id, [FromBody] UpdateUserDto dto)
        => Ok(await _userService.UpdateAsync(id, dto));

    // DELETE /api/users/{id} – soft-delete konto
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _userService.DeleteAsync(id);
        return NoContent();
    }
}

// ─── VenuesController ─────────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class VenuesController : ControllerBase
{
    private readonly IVenueService _venueService;
    private readonly IValidator<CreateVenueDto> _validator;

    public VenuesController(IVenueService venueService, IValidator<CreateVenueDto> validator)
    {
        _venueService = venueService;
        _validator = validator;
    }

    // GET /api/venues – alla venues
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VenueDto>>> GetAll()
        => Ok(await _venueService.GetAllAsync());

    // GET /api/venues/search?city=Stockholm – sök på stad
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<VenueDto>>> Search(
        [FromQuery] string city, [FromQuery] string? country)
        => Ok(await _venueService.SearchByLocationAsync(city, country));

    // GET /api/venues/{id} – hämta en venue
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<VenueDto>> GetById(Guid id)
    {
        var venue = await _venueService.GetByIdAsync(id);
        return venue == null ? NotFound() : Ok(venue);
    }

    // POST /api/venues – skapa ny venue
    [HttpPost]
    public async Task<ActionResult<VenueDto>> Create([FromBody] CreateVenueDto dto)
    {
        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var created = await _venueService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT /api/venues/{id} – uppdatera venue
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<VenueDto>> Update(Guid id, [FromBody] UpdateVenueDto dto)
        => Ok(await _venueService.UpdateAsync(id, dto));

    // DELETE /api/venues/{id} – soft-delete venue
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _venueService.DeleteAsync(id);
        return NoContent();
    }
}

// ─── RegistrationsController ──────────────────────────────────
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RegistrationsController : ControllerBase
{
    private readonly IRegistrationService _registrationService;
    private readonly IValidator<CreateRegistrationDto> _validator;

    public RegistrationsController(IRegistrationService registrationService, IValidator<CreateRegistrationDto> validator)
    {
        _registrationService = registrationService;
        _validator = validator;
    }

    // GET /api/registrations/event/{id} – alla deltagare för ett event
    [HttpGet("event/{eventId:guid}")]
    public async Task<ActionResult<IEnumerable<RegistrationDto>>> GetByEvent(Guid eventId)
        => Ok(await _registrationService.GetByEventAsync(eventId));

    // GET /api/registrations/user/{id} – en användares bokningar
    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<IEnumerable<RegistrationDto>>> GetByUser(Guid userId)
        => Ok(await _registrationService.GetByUserAsync(userId));

    // POST /api/registrations – boka ett event
    [HttpPost]
    public async Task<ActionResult<RegistrationDto>> Register([FromBody] CreateRegistrationDto dto)
    {
        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var result = await _registrationService.RegisterAsync(dto);
        return CreatedAtAction(nameof(GetByEvent), new { eventId = result.EventId }, result);
    }

    // PATCH /api/registrations/{id}/checkin – checka in deltagare
    [HttpPatch("{id:guid}/checkin")]
    public async Task<ActionResult<RegistrationDto>> CheckIn(Guid id)
        => Ok(await _registrationService.CheckInAsync(id));

    // PATCH /api/registrations/{id}/status – ändra status manuellt
    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<RegistrationDto>> UpdateStatus(Guid id, [FromBody] UpdateRegistrationStatusDto dto)
        => Ok(await _registrationService.UpdateStatusAsync(id, dto));

    // DELETE /api/registrations/{id} – avboka
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        await _registrationService.CancelAsync(id);
        return NoContent();
    }
}

// ─── ReviewsController ────────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly IValidator<CreateReviewDto> _validator;

    public ReviewsController(IReviewService reviewService, IValidator<CreateReviewDto> validator)
    {
        _reviewService = reviewService;
        _validator = validator;
    }

    // GET /api/reviews/event/{id} – alla recensioner för ett event
    [HttpGet("event/{eventId:guid}")]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetByEvent(Guid eventId)
        => Ok(await _reviewService.GetByEventAsync(eventId));

    // POST /api/reviews – skapa recension
    [HttpPost]
    public async Task<ActionResult<ReviewDto>> Create([FromBody] CreateReviewDto dto)
    {
        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var result = await _reviewService.CreateAsync(dto);
        return Created($"/api/reviews/event/{dto.EventId}", result);
    }

    // DELETE /api/reviews/{id} – ta bort recension
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _reviewService.DeleteAsync(id);
        return NoContent();
    }
}