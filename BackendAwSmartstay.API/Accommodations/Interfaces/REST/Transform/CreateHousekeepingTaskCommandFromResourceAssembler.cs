using BackendAwSmartstay.API.Accommodations.Domain.Model.Commands;
using BackendAwSmartstay.API.Accommodations.Domain.Model.ValueObjects;
using BackendAwSmartstay.API.Accommodations.Interfaces.REST.Resources;

namespace BackendAwSmartstay.API.Accommodations.Interfaces.REST.Transform;

/// <summary>
/// Assembler to convert a <see cref="CreateHousekeepingTaskResource"/> to a <see cref="CreateHousekeepingTaskCommand"/>.
/// Transforms the TaskType string to the strongly-typed <see cref="HousekeepingTaskType"/> enum.
/// </summary>
public static class CreateHousekeepingTaskCommandFromResourceAssembler
{
    public static CreateHousekeepingTaskCommand ToCommandFromResource(CreateHousekeepingTaskResource resource)
    {
        return new CreateHousekeepingTaskCommand(
            resource.RoomId,
            resource.AssignedHousekeeperId,
            Enum.TryParse<HousekeepingTaskType>(resource.TaskType, ignoreCase: true, out var taskType)
                ? taskType
                : HousekeepingTaskType.Routine
        );
    }
}
