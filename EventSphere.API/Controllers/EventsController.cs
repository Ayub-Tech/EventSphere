using EventSphere.Application.DTOs.Event;
using EventSphere.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace EventSphere.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;
    private readonly IValidator<CreateEventDto> _createValidator;
    private readonly ILogger<EventsController> _logger;

    public EventsController(
        IEventService eventService,
        IValidator<CreateEventDto> createValidator,
        ILogger<EventsController> logger)
    {
        _eventService = eventService;
        _createValidator = createValidator;
        _logger = logger;
    }

    // GET /api/events – hämta filtrerade och paginerade events
    [HttpGet]
    public async Task<ActionResult<PagedEventResult>> GetEvents([FromQuery] EventQueryParams query)
    {
        var result = await _eventService.GetFilteredAsync(query);
        return Ok(result);
    }

    // GET /api/events/upcoming – närmaste kommande events
    [HttpGet("upcoming")]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetUpcoming([FromQuery] int count = 10)
        => Ok(await _eventService.GetUpcomingAsync(count));

    // GET /api/events/organizer/{id} – events av en specifik organisatör
    [HttpGet("organizer/{organizerId:guid}")]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetByOrganizer(Guid organizerId)
        => Ok(await _eventService.GetByOrganizerAsync(organizerId));

    // GET /api/events/{id} – hämta ett specifikt event
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EventDto>> GetById(Guid id)
    {
        var ev = await _eventService.GetByIdAsync(id);
        return ev == null ? NotFound(new { message = $"Event {id} not found." }) : Ok(ev);
    }

    // POST /api/events – skapa nytt event
    [HttpPost]
    public async Task<ActionResult<EventDto>> Create([FromBody] CreateEventDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var created = await _eventService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT /api/events/{id} – uppdatera ett event
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<EventDto>> Update(Guid id, [FromBody] UpdateEventDto dto)
        => Ok(await _eventService.UpdateAsync(id, dto));

    // PATCH /api/events/{id}/publish – publicera ett event
    [HttpPatch("{id:guid}/publish")]
    public async Task<ActionResult<EventDto>> Publish(Guid id)
        => Ok(await _eventService.PublishAsync(id));

    // PATCH /api/events/{id}/cancel – avboka ett event
    [HttpPatch("{id:guid}/cancel")]
    public async Task<ActionResult<EventDto>> Cancel(Guid id)
        => Ok(await _eventService.CancelAsync(id));

    // DELETE /api/events/{id} – soft-delete ett event
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _eventService.DeleteAsync(id);
        return NoContent();
    }
}