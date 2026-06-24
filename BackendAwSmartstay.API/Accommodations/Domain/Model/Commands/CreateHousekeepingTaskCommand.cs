using BackendAwSmartstay.API.Accommodations.Domain.Model.ValueObjects;

namespace BackendAwSmartstay.API.Accommodations.Domain.Model.Commands;

/// <summary>
/// Command to create a new housekeeping task for a specific room.
/// </summary>
/// <param name="RoomId">The identifier of the room to clean.</param>
/// <param name="AssignedHousekeeperId">The identifier of the assigned housekeeper.</param>
/// <param name="TaskType">The type of cleaning task (Routine, DeepClean, TouchUp).</param>
public record CreateHousekeepingTaskCommand(
    int RoomId,
    int AssignedHousekeeperId,
    HousekeepingTaskType TaskType);
