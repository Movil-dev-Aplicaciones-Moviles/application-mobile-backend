using BackendAwSmartstay.API.Shared.Domain.Model.Events;

namespace BackendAwSmartstay.API.Accommodations.Domain.Model.Events;

/// <summary>
/// Domain event raised when maintenance work on a room has started.
/// Triggers the Room aggregate to update its status to <see cref="ValueObjects.RoomStatus.Maintenance"/>.
/// </summary>
/// <param name="RoomId">The identifier of the room under maintenance.</param>
public record MaintenanceStartedEvent(int RoomId) : IEvent;
