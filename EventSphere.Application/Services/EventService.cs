using EventSphere.Application.DTOs.Event;
using EventSphere.Application.Interfaces;
using EventSphere.Application.Mappings;
using EventSphere.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EventSphere.Application.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IRegistrationRepository _registrationRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly ILogger<EventService> _logger;

    public EventService(
        IEventRepository eventRepository,
        IRegistrationRepository registrationRepository,
        IReviewRepository reviewRepository,
        ILogger<EventService> logger)
    {
        _eventRepository = eventRepository;
        _registrationRepository = registrationRepository;
        _reviewRepository = reviewRepository;
        _logger = logger;
    }

    public async Task<PagedEventResult> GetFilteredAsync(EventQueryParams query)
    {
        var events = await _eventRepository.GetFilteredAsync(
            query.Search, query.CategoryId, query.City,
            query.FromDate, query.ToDate, query.MaxPrice,
            query.Status, query.SortBy, query.Ascending,
            query.Page, query.PageSize);

        var totalCount = await _eventRepository.GetTotalCountAsync(
            query.Search, query.CategoryId, query.City, query.Status);

        // Bygg DTO-lista med boknings- och recensionsdata per event
        var eventDtos = new List<EventDto>();
        foreach (var ev in events)
        {
            var count = await _registrationRepository.GetRegistrationCountAsync(ev.Id);
            var avgRating = await _reviewRepository.GetAverageRatingAsync(ev.Id);
            var reviewCount = (await _reviewRepository.GetByEventAsync(ev.Id)).Count();
            eventDtos.Add(ev.ToDto(count, avgRating, reviewCount));
        }

        return new PagedEventResult
        {
            Items = eventDtos,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<EventDto?> GetByIdAsync(Guid id)
    {
        var ev = await _eventRepository.GetWithDetailsAsync(id);
        if (ev == null) return null;

        var count = await _registrationRepository.GetRegistrationCountAsync(id);
        var avgRating = await _reviewRepository.GetAverageRatingAsync(id);
        var reviewCount = (await _reviewRepository.GetByEventAsync(id)).Count();

        return ev.ToDto(count, avgRating, reviewCount);
    }

    public async Task<IEnumerable<EventDto>> GetUpcomingAsync(int count = 10)
    {
        var events = await _eventRepository.GetUpcomingAsync(count);
        var result = new List<EventDto>();
        foreach (var ev in events)
        {
            var attendeeCount = await _registrationRepository.GetRegistrationCountAsync(ev.Id);
            result.Add(ev.ToDto(attendeeCount));
        }
        return result;
    }

    public async Task<IEnumerable<EventDto>> GetByOrganizerAsync(Guid organizerId)
    {
        var events = await _eventRepository.GetByOrganizerAsync(organizerId);
        var result = new List<EventDto>();
        foreach (var ev in events)
        {
            var count = await _registrationRepository.GetRegistrationCountAsync(ev.Id);
            result.Add(ev.ToDto(count));
        }
        return result;
    }

    public async Task<EventDto> CreateAsync(CreateEventDto dto)
    {
        _logger.LogInformation("Creating event: {Title}", dto.Title);
        var entity = dto.ToEntity();
        var created = await _eventRepository.AddAsync(entity);
        var withDetails = await _eventRepository.GetWithDetailsAsync(created.Id);
        return withDetails!.ToDto();
    }

    public async Task<EventDto> UpdateAsync(Guid id, UpdateEventDto dto)
    {
        // Kastar KeyNotFoundException om eventet inte finns → middleware returnerar 404
        var entity = await _eventRepository.GetWithDetailsAsync(id)
            ?? throw new KeyNotFoundException($"Event {id} not found.");

        // Uppdatera bara fält som faktiskt skickats (ej null)
        if (dto.Title != null) entity.Title = dto.Title;
        if (dto.Description != null) entity.Description = dto.Description;
        if (dto.StartDate.HasValue) entity.StartDate = dto.StartDate.Value;
        if (dto.EndDate.HasValue) entity.EndDate = dto.EndDate.Value;
        if (dto.Price.HasValue) entity.Price = dto.Price.Value;
        if (dto.MaxAttendees.HasValue) entity.MaxAttendees = dto.MaxAttendees.Value;
        if (dto.ImageUrl != null) entity.ImageUrl = dto.ImageUrl;
        if (dto.IsOnline.HasValue) entity.IsOnline = dto.IsOnline.Value;
        if (dto.OnlineUrl != null) entity.OnlineUrl = dto.OnlineUrl;
        if (dto.Tags != null) entity.Tags = dto.Tags;
        if (dto.Status != null) entity.Status = dto.Status;
        if (dto.VenueId.HasValue) entity.VenueId = dto.VenueId.Value;
        if (dto.CategoryId.HasValue) entity.CategoryId = dto.CategoryId.Value;

        entity.UpdatedAt = DateTime.UtcNow;
        await _eventRepository.UpdateAsync(entity);

        var updated = await _eventRepository.GetWithDetailsAsync(id);
        var count = await _registrationRepository.GetRegistrationCountAsync(id);
        return updated!.ToDto(count);
    }

    public async Task DeleteAsync(Guid id)
    {
        _logger.LogInformation("Deleting event {Id}", id);
        await _eventRepository.DeleteAsync(id);
    }

    // Publicera och avboka är bara statusändringar
    public async Task<EventDto> PublishAsync(Guid id)
        => await UpdateAsync(id, new UpdateEventDto { Status = "Published" });

    public async Task<EventDto> CancelAsync(Guid id)
        => await UpdateAsync(id, new UpdateEventDto { Status = "Cancelled" });
}