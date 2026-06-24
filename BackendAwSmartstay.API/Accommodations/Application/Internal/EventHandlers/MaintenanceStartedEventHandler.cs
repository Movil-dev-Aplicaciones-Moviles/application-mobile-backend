using BackendAwSmartstay.API.Accommodations.Domain.Model.Aggregates;
using BackendAwSmartstay.API.Accommodations.Domain.Model.Events;
using BackendAwSmartstay.API.Accommodations.Domain.Model.ValueObjects;
using BackendAwSmartstay.API.Accommodations.Domain.Repositories;
using BackendAwSmartstay.API.Shared.Application.Internal.EventHandlers;

namespace BackendAwSmartstay.API.Accommodations.Application.Internal.EventHandlers;

/// <summary>
/// Handles the <see cref="MaintenanceStartedEvent"/> by updating the corresponding
/// Room aggregate's status to <see cref="RoomStatus.Maintenance"/> in memory.
/// Does NOT call <c>unitOfWork.CompleteAsync()</c> — the originating UnitOfWork
/// persists all changes atomically after all handlers finish.
/// </summary>
public class MaintenanceStartedEventHandler(
    IRoomRepository roomRepository) : IEventHandler<MaintenanceStartedEvent>
{
    /// <inheritdoc />
    public async Task Handle(MaintenanceStartedEvent notification, CancellationToken cancellationToken)
    {
        var room = await roomRepository.FindByIdAsync(notification.RoomId);
        if (room is null) return;

        room.UpdateInformation(
            room.RoomTypeId,
            room.Price,
            room.Description,
            room.Amenities,
            RoomStatus.Maintenance);

        roomRepository.Update(room);
    }
}
