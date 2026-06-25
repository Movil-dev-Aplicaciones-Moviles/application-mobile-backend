using BackendAwSmartstay.API.Bookings.Domain.Repositories;
using BackendAwSmartstay.API.Payments.Domain.Model.Events;
using BackendAwSmartstay.API.Shared.Application.Internal.EventHandlers;
using BackendAwSmartstay.API.Shared.Domain.Repositories;

namespace BackendAwSmartstay.API.Bookings.Application.Internal.EventHandlers;

/// <summary>
/// Handles the <see cref="PaymentCompletedEvent"/> by confirming the associated booking.
/// This handler bridges the Payments and Bookings bounded contexts via domain events.
/// </summary>
public class PaymentCompletedEventHandler(
    IBookingRepository bookingRepository,
    IUnitOfWork unitOfWork)
    : IEventHandler<PaymentCompletedEvent>
{
    /// <inheritdoc />
    public async Task Handle(PaymentCompletedEvent notification, CancellationToken cancellationToken)
    {
        var booking = await bookingRepository.FindByIdAsync(notification.BookingId);
        
        if (booking is null)
        {
            throw new InvalidOperationException(
                $"Booking #{notification.BookingId} not found. Cannot confirm a booking that does not exist.");
        }

        booking.Confirm();
        bookingRepository.Update(booking);
        await unitOfWork.CompleteAsync();
    }
}
