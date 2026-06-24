namespace BackendAwSmartstay.API.Accommodations.Domain.Model.Commands;

/// <summary>
/// Command to mark an existing maintenance request as resolved.
/// </summary>
/// <param name="RequestId">The unique identifier of the request to resolve.</param>
public record ResolveMaintenanceCommand(int RequestId);
