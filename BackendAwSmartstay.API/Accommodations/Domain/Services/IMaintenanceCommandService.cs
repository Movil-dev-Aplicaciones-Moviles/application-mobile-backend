using BackendAwSmartstay.API.Accommodations.Domain.Model.Aggregates;
using BackendAwSmartstay.API.Accommodations.Domain.Model.Commands;

namespace BackendAwSmartstay.API.Accommodations.Domain.Services;

/// <summary>
/// Defines the contract for services that handle maintenance request state changes.
/// </summary>
public interface IMaintenanceCommandService
{
    /// <summary>
    /// Handles the creation of a new maintenance request.
    /// </summary>
    Task<MaintenanceRequest?> Handle(CreateMaintenanceRequestCommand command);

    /// <summary>
    /// Handles starting maintenance work on a request.
    /// </summary>
    Task<MaintenanceRequest?> Handle(StartMaintenanceCommand command);

    /// <summary>
    /// Handles resolving a maintenance request.
    /// </summary>
    Task<MaintenanceRequest?> Handle(ResolveMaintenanceCommand command);
}
