namespace BackendAwSmartstay.API.Accommodations.Interfaces.REST.Resources;

/// <summary>
/// Resource for creating a new housekeeping task.
/// The client sends TaskType as a string (e.g., "Routine", "DeepClean", "TouchUp");
/// the Assembler transforms it to the strongly-typed <see cref="Domain.Model.ValueObjects.HousekeepingTaskType"/>.
/// </summary>
/// <param name="RoomId">The identifier of the room to clean.</param>
/// <param name="AssignedHousekeeperId">The identifier of the assigned housekeeper.</param>
/// <param name="TaskType">The type of cleaning task as a string.</param>
public record CreateHousekeepingTaskResource(
    int RoomId,
    int AssignedHousekeeperId,
    string TaskType);
