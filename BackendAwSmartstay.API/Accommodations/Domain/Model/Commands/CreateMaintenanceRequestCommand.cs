using BackendAwSmartstay.API.Accommodations.Domain.Model.ValueObjects;

namespace BackendAwSmartstay.API.Accommodations.Domain.Model.Commands;

/// <summary>
/// Command to create a new maintenance request for a specific room.
/// </summary>
/// <param name="RoomId">The identifier of the room requiring maintenance.</param>
/// <param name="Description">A description of the issue.</param>
/// <param name="Priority">The priority level (Low, Medium, High, Urgent).</param>
public record CreateMaintenanceRequestCommand(
    int RoomId,
    string Description,
    MaintenancePriority Priority);
