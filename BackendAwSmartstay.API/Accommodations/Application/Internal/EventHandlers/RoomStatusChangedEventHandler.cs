using BackendAwSmartstay.API.Accommodations.Domain.Model.Events;
using BackendAwSmartstay.API.Shared.Application.Internal.EventHandlers;
using Microsoft.Extensions.Logging;

namespace BackendAwSmartstay.API.Accommodations.Application.Internal.EventHandlers;

/// <summary>
/// Handles the <see cref="RoomStatusChangedEvent"/> by logging the status transition.
/// Demonstrates that the domain event pipeline is operational.
/// </summary>
public class RoomStatusChangedEventHandler(ILogger<RoomStatusChangedEventHandler> logger)
    : IEventHandler<RoomStatusChangedEvent>
{
    /// <inheritdoc />
    public Task Handle(RoomStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Room {RoomId} changed status from {OldStatus} to {NewStatus}",
            notification.RoomId,
            notification.OldStatus,
            notification.NewStatus);

        return Task.CompletedTask;
    }
}
