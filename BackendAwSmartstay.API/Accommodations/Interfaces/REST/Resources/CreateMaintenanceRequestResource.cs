namespace BackendAwSmartstay.API.Accommodations.Interfaces.REST.Resources;

/// <summary>
/// Resource for creating a new maintenance request.
/// The client sends Priority as a string (e.g., "Low", "Medium", "High", "Urgent");
/// the Assembler transforms it to the strongly-typed <see cref="Domain.Model.ValueObjects.MaintenancePriority"/>.
/// </summary>
/// <param name="RoomId">The identifier of the room requiring maintenance.</param>
/// <param name="Description">A description of the issue.</param>
/// <param name="Priority">The priority level as a string.</param>
public record CreateMaintenanceRequestResource(
    int RoomId,
    string Description,
    string Priority);
