using BackendAwSmartstay.API.Accommodations.Domain.Model.Commands;
using BackendAwSmartstay.API.Accommodations.Domain.Model.ValueObjects;
using BackendAwSmartstay.API.Accommodations.Interfaces.REST.Resources;

namespace BackendAwSmartstay.API.Accommodations.Interfaces.REST.Transform;

/// <summary>
/// Assembler to convert a <see cref="CreateMaintenanceRequestResource"/> to a <see cref="CreateMaintenanceRequestCommand"/>.
/// Transforms the Priority string to the strongly-typed <see cref="MaintenancePriority"/> enum.
/// </summary>
public static class CreateMaintenanceRequestCommandFromResourceAssembler
{
    public static CreateMaintenanceRequestCommand ToCommandFromResource(CreateMaintenanceRequestResource resource)
    {
        return new CreateMaintenanceRequestCommand(
            resource.RoomId,
            resource.Description,
            Enum.TryParse<MaintenancePriority>(resource.Priority, ignoreCase: true, out var priority)
                ? priority
                : MaintenancePriority.Low
        );
    }
}
