namespace BackendAwSmartstay.API.Accommodations.Domain.Model.Commands;

/// <summary>
/// Command to start maintenance work on an existing request.
/// </summary>
/// <param name="RequestId">The unique identifier of the request to start.</param>
/// <param name="TechnicianId">The identifier of the assigned technician.</param>
public record StartMaintenanceCommand(int RequestId, int TechnicianId);
