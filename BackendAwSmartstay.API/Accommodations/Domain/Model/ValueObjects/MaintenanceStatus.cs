namespace BackendAwSmartstay.API.Accommodations.Domain.Model.ValueObjects;

/// <summary>
/// Represents the lifecycle status of a maintenance request.
/// </summary>
public enum MaintenanceStatus
{
    /// <summary>
    /// The request has been created but not yet started.
    /// </summary>
    Pending,
    
    /// <summary>
    /// The maintenance work is currently in progress.
    /// </summary>
    InProgress,
    
    /// <summary>
    /// The maintenance work has been completed.
    /// </summary>
    Resolved,
    
    /// <summary>
    /// The request has been closed after verification.
    /// </summary>
    Closed
}
