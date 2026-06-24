using BackendAwSmartstay.API.Shared.Domain.Model.Events;

namespace BackendAwSmartstay.API.Accommodations.Domain.Model.Events;

/// <summary>
/// Domain event raised when a new room is created.
/// Implements <see cref="IEvent"/> for integration with the Cortex.Mediator pipeline.
/// </summary>
/// <param name="RoomId">The unique identifier of the created room.</param>
/// <param name="HotelId">The identifier of the hotel the room belongs to.</param>
/// <param name="RoomTypeId">The identifier of the room type assigned to the room.</param>
public record RoomCreatedEvent(
    int RoomId,
    int HotelId,
    int RoomTypeId) : IEvent;
