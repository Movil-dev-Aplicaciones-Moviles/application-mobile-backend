using BackendAwSmartstay.API.Shared.Domain.Model.Events;

namespace BackendAwSmartstay.API.Payments.Domain.Model.Events;

/// <summary>
/// Domain event raised when a payment is successfully completed.
/// Consumed by other bounded contexts (e.g., Bookings) to react accordingly.
/// Implements <see cref="IEvent"/> for integration with the Cortex.Mediator pipeline.
/// </summary>
/// <param name="PaymentId">The unique identifier of the completed payment.</param>
/// <param name="BookingId">The identifier of the booking associated with the payment.</param>
public record PaymentCompletedEvent(
    int PaymentId,
    int BookingId) : IEvent;
