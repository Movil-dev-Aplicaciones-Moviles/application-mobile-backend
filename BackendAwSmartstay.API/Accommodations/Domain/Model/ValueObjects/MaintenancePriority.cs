namespace BackendAwSmartstay.API.Accommodations.Domain.Model.ValueObjects;

/// <summary>
/// Represents the priority level of a maintenance request.
/// </summary>
public enum MaintenancePriority
{
    /// <summary>
    /// Non-urgent, can be scheduled.
    /// </summary>
    Low,
    
    /// <summary>
    /// Should be addressed within a reasonable timeframe.
    /// </summary>
    Medium,
    
    /// <summary>
    /// Requires prompt attention.
    /// </summary>
    High,
    
    /// <summary>
    /// Immediate action required (e.g., gas leak, water leak).
    /// </summary>
    Urgent
}
