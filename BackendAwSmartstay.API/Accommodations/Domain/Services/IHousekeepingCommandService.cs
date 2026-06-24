using BackendAwSmartstay.API.Accommodations.Domain.Model.Aggregates;
using BackendAwSmartstay.API.Accommodations.Domain.Model.Commands;

namespace BackendAwSmartstay.API.Accommodations.Domain.Services;

/// <summary>
/// Defines the contract for services that handle housekeeping task state changes.
/// </summary>
public interface IHousekeepingCommandService
{
    /// <summary>
    /// Handles the creation of a new housekeeping task.
    /// </summary>
    /// <param name="command">The create command.</param>
    /// <returns>The created task or null if creation failed.</returns>
    Task<HousekeepingTask?> Handle(CreateHousekeepingTaskCommand command);

    /// <summary>
    /// Handles the completion of an existing housekeeping task.
    /// </summary>
    /// <param name="command">The complete command.</param>
    /// <returns>The completed task or null if not found.</returns>
    Task<HousekeepingTask?> Handle(CompleteHousekeepingTaskCommand command);
}
