using BackendAwSmartstay.API.Accommodations.Domain.Model.Aggregates;
using BackendAwSmartstay.API.Accommodations.Domain.Model.Events;
using BackendAwSmartstay.API.Accommodations.Domain.Model.ValueObjects;
using BackendAwSmartstay.API.Accommodations.Domain.Repositories;
using BackendAwSmartstay.API.Shared.Application.Internal.EventHandlers;

namespace BackendAwSmartstay.API.Accommodations.Application.Internal.EventHandlers;

/// <summary>
/// Handles the <see cref="MaintenanceResolvedEvent"/> by updating the corresponding
/// Room aggregate's status to <see cref="RoomStatus.Dirty"/> in memory.
/// After maintenance, the room needs to be cleaned before it can be made available.
/// Does NOT call <c>unitOfWork.CompleteAsync()</c> — the originating UnitOfWork
/// persists all changes atomically after all handlers finish.
/// </summary>
public class MaintenanceResolvedEventHandler(
    IRoomRepository roomRepository) : IEventHandler<MaintenanceResolvedEvent>
{
    /// <inheritdoc />
    public async Task Handle(MaintenanceResolvedEvent notification, CancellationToken cancellationToken)
    {
        var room = await roomRepository.FindByIdAsync(notification.RoomId);
        if (room is null) return;

        room.UpdateInformation(
            room.RoomTypeId,
            room.Price,
            room.Description,
            room.Amenities,
            RoomStatus.Dirty);

        roomRepository.Update(room);
    }
}
