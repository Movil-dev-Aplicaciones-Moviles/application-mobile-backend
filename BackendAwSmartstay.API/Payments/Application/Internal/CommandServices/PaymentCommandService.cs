using BackendAwSmartstay.API.Payments.Domain.Model.Aggregates;
using BackendAwSmartstay.API.Payments.Domain.Model.Commands;
using BackendAwSmartstay.API.Payments.Domain.Repositories;
using BackendAwSmartstay.API.Payments.Domain.Services;
using BackendAwSmartstay.API.Shared.Domain.Repositories;

namespace BackendAwSmartstay.API.Payments.Application.Internal.CommandServices;

/// <summary>
/// Implementation of the payment command service.
/// Orchestrates the payment process without coupling to other bounded contexts.
/// Domain events are emitted by the Payment aggregate for downstream consumers.
/// </summary>
public class PaymentCommandService(
    IPaymentRepository paymentRepository,
    IUnitOfWork unitOfWork) 
    : IPaymentCommandService
{
    /// <summary>
    /// Processes a payment command, simulating bank validation.
    /// On success, the payment is completed and a PaymentCompletedEvent is emitted.
    /// </summary>
    /// <param name="command">The command containing the payment data.</param>
    /// <returns>The processed payment or null if processing failed.</returns>
    public async Task<Payment?> Handle(ProcessPaymentCommand command)
    {
        // 1. Start the payment process (Pending Status)
        var payment = new Payment(command);

        // 2. Simulation Logic (Fake Gateway)
        // Note: Structural validation of Amount is now handled by the Money Value Object.
        // Note: Structural validation of CardNumber is now handled by the CreditCard Value Object.
        bool isApproved = true;

        // TODO: Move to a Domain Service FakeGateway for external gateway simulation.
        // This simulates a declined card by the bank (external system failure),
        // NOT a structural validation of the data.
        if (command.CardNumber.EndsWith("0000")) isApproved = false; // Simulate bank decline

        if (isApproved)
        {
            payment.Complete(); // Change payment status to Completed and emit PaymentCompletedEvent
            Console.WriteLine($"Payment #{payment.Id} completed for Booking #{payment.BookingId}.");
        }
        else
        {
            payment.Fail(); // Change payment status to Failed (2)
            Console.WriteLine("Payment declined by simulation logic.");
        }

        // 3. Save payment (UnitOfWork will dispatch any domain events)
        await paymentRepository.AddAsync(payment);
        await unitOfWork.CompleteAsync();

        return payment;
    }
}