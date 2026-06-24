using BackendAwSmartstay.API.Shared.Domain.Model.Events;

namespace BackendAwSmartstay.API.Accommodations.Domain.Model.Events;

/// <summary>
/// Domain event raised when maintenance work on a room has been resolved.
/// Triggers the Room aggregate to update its status to <see cref="ValueObjects.RoomStatus.Dirty"/>,
/// requiring housekeeping before the room can be made available again.
/// </summary>
/// <param name="RoomId">The identifier of the room that was repaired.</param>
public record MaintenanceResolvedEvent(int RoomId) : IEvent;
