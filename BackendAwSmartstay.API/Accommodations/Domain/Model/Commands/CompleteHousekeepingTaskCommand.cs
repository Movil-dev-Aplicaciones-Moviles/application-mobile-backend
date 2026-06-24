namespace BackendAwSmartstay.API.Accommodations.Domain.Model.Commands;

/// <summary>
/// Command to mark an existing housekeeping task as completed.
/// </summary>
/// <param name="TaskId">The unique identifier of the task to complete.</param>
public record CompleteHousekeepingTaskCommand(int TaskId);
