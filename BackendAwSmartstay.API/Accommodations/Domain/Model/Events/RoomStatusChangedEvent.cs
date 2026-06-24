using BackendAwSmartstay.API.Accommodations.Domain.Model.ValueObjects;
using BackendAwSmartstay.API.Shared.Domain.Model.Events;

namespace BackendAwSmartstay.API.Accommodations.Domain.Model.Events;

/// <summary>
/// Domain event raised when a room's operational status changes.
/// Implements <see cref="IEvent"/> for integration with the Cortex.Mediator pipeline.
/// </summary>
/// <param name="RoomId">The unique identifier of the room.</param>
/// <param name="OldStatus">The previous room status.</param>
/// <param name="NewStatus">The new room status.</param>
public record RoomStatusChangedEvent(
    int RoomId,
    RoomStatus OldStatus,
    RoomStatus NewStatus) : IEvent;
