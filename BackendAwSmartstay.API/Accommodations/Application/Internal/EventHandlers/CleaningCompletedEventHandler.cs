using BackendAwSmartstay.API.Accommodations.Domain.Model.Aggregates;
using BackendAwSmartstay.API.Accommodations.Domain.Model.Events;
using BackendAwSmartstay.API.Accommodations.Domain.Model.ValueObjects;
using BackendAwSmartstay.API.Accommodations.Domain.Repositories;
using BackendAwSmartstay.API.Shared.Application.Internal.EventHandlers;

namespace BackendAwSmartstay.API.Accommodations.Application.Internal.EventHandlers;

/// <summary>
/// Handles the <see cref="CleaningCompletedEvent"/> by updating the corresponding
/// Room aggregate's status to <see cref="RoomStatus.Clean"/> in memory.
/// Does NOT call <c>unitOfWork.CompleteAsync()</c> because this handler runs inside
/// the UnitOfWork's event publishing loop — the originating <c>CompleteAsync()</c>
/// will persist all changes (task + room) atomically after all handlers finish.
/// </summary>
public class CleaningCompletedEventHandler(
    IRoomRepository roomRepository) : IEventHandler<CleaningCompletedEvent>
{
    /// <inheritdoc />
    public async Task Handle(CleaningCompletedEvent notification, CancellationToken cancellationToken)
    {
        var room = await roomRepository.FindByIdAsync(notification.RoomId);
        if (room is null) return;

        // Mutate the Room aggregate in memory — EF Core ChangeTracker picks this up
        room.UpdateInformation(
            room.RoomTypeId,
            room.Price,
            room.Description,
            room.Amenities,
            RoomStatus.Clean);

        roomRepository.Update(room);

        // ⚠️ NO llamar a unitOfWork.CompleteAsync() aquí.
        // El UnitOfWork que publicó este evento ejecutará SaveChangesAsync()
        // inmediatamente después de que este handler retorne, persistiendo
        // tanto el cambio en housekeeping_tasks como en rooms en una sola transacción.
    }
}
