using BackendAwSmartstay.API.Accommodations.Domain.Model.Aggregates;
using BackendAwSmartstay.API.Accommodations.Domain.Model.Commands;
using BackendAwSmartstay.API.Accommodations.Domain.Repositories;
using BackendAwSmartstay.API.Accommodations.Domain.Services;
using BackendAwSmartstay.API.Shared.Domain.Repositories;

namespace BackendAwSmartstay.API.Accommodations.Application.Internal.CommandServices;

/// <summary>
/// Service implementation for handling housekeeping task commands.
/// Orchestrates the flow between the repository, domain logic, and unit of work.
/// </summary>
public class HousekeepingCommandService(
    IHousekeepingTaskRepository housekeepingTaskRepository,
    IUnitOfWork unitOfWork) : IHousekeepingCommandService
{
    /// <inheritdoc />
    public async Task<HousekeepingTask?> Handle(CreateHousekeepingTaskCommand command)
    {
        var task = new HousekeepingTask(
            command.RoomId,
            command.AssignedHousekeeperId,
            command.TaskType);

        await housekeepingTaskRepository.AddAsync(task);
        await unitOfWork.CompleteAsync();
        return task;
    }

    /// <inheritdoc />
    public async Task<HousekeepingTask?> Handle(CompleteHousekeepingTaskCommand command)
    {
        var task = await housekeepingTaskRepository.FindByIdAsync(command.TaskId);
        if (task is null) return null;

        task.CompleteTask();
        housekeepingTaskRepository.Update(task);
        await unitOfWork.CompleteAsync();
        return task;
    }
}
