using BackendAwSmartstay.API.Shared.Domain.Model.Events;

namespace BackendAwSmartstay.API.Accommodations.Domain.Model.Events;

/// <summary>
/// Domain event raised when a housekeeping task is completed.
/// Triggers the Room aggregate to update its status to Clean.
/// </summary>
/// <param name="RoomId">The identifier of the room that was cleaned.</param>
public record CleaningCompletedEvent(int RoomId) : IEvent;
