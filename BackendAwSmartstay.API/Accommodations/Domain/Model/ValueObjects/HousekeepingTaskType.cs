namespace BackendAwSmartstay.API.Accommodations.Domain.Model.ValueObjects;

/// <summary>
/// Represents the type or depth of a housekeeping task.
/// </summary>
public enum HousekeepingTaskType
{
    /// <summary>
    /// Standard daily cleaning (make bed, take out trash, wipe surfaces).
    /// </summary>
    Routine,
    
    /// <summary>
    /// Intensive cleaning (shampoo carpets, wash curtains, deep scrub).
    /// </summary>
    DeepClean,
    
    /// <summary>
    /// Quick refresh (replace towels, restock amenities, quick vacuum).
    /// </summary>
    TouchUp
}
