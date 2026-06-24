namespace BackendAwSmartstay.API.Accommodations.Domain.Model.ValueObjects;

/// <summary>
/// Represents the lifecycle status of a housekeeping task.
/// </summary>
public enum HousekeepingTaskStatus
{
    /// <summary>
    /// The task has been created but not yet started.
    /// </summary>
    Pending,
    
    /// <summary>
    /// The task is currently being performed.
    /// </summary>
    InProgress,
    
    /// <summary>
    /// The task has been completed by the housekeeper.
    /// </summary>
    Completed,
    
    /// <summary>
    /// The completed task has been inspected and approved.
    /// </summary>
    Inspected
}
