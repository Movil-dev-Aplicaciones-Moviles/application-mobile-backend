using BackendAwSmartstay.API.Accommodations.Domain.Model.Aggregates;
using BackendAwSmartstay.API.Accommodations.Domain.Model.Commands;
using BackendAwSmartstay.API.Accommodations.Domain.Repositories;
using BackendAwSmartstay.API.Accommodations.Domain.Services;
using BackendAwSmartstay.API.Shared.Domain.Repositories;

namespace BackendAwSmartstay.API.Accommodations.Application.Internal.CommandServices;

/// <summary>
/// Service implementation for handling maintenance request commands.
/// Orchestrates the flow between the repository, domain logic, and unit of work.
/// </summary>
public class MaintenanceCommandService(
    IMaintenanceRequestRepository maintenanceRequestRepository,
    IUnitOfWork unitOfWork) : IMaintenanceCommandService
{
    /// <inheritdoc />
    public async Task<MaintenanceRequest?> Handle(CreateMaintenanceRequestCommand command)
    {
        var request = new MaintenanceRequest(
            command.RoomId,
            command.Description,
            command.Priority);

        await maintenanceRequestRepository.AddAsync(request);
        await unitOfWork.CompleteAsync();
        return request;
    }

    /// <inheritdoc />
    public async Task<MaintenanceRequest?> Handle(StartMaintenanceCommand command)
    {
        var request = await maintenanceRequestRepository.FindByIdAsync(command.RequestId);
        if (request is null) return null;

        request.StartMaintenance(command.TechnicianId);
        maintenanceRequestRepository.Update(request);
        await unitOfWork.CompleteAsync();
        return request;
    }

    /// <inheritdoc />
    public async Task<MaintenanceRequest?> Handle(ResolveMaintenanceCommand command)
    {
        var request = await maintenanceRequestRepository.FindByIdAsync(command.RequestId);
        if (request is null) return null;

        request.ResolveMaintenance();
        maintenanceRequestRepository.Update(request);
        await unitOfWork.CompleteAsync();
        return request;
    }
}
