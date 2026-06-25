using BackendAwSmartstay.API.Payments.Domain.Model.Commands;
using BackendAwSmartstay.API.Payments.Domain.Model.Events;
using BackendAwSmartstay.API.Payments.Domain.Model.ValueObjects;

namespace BackendAwSmartstay.API.Payments.Domain.Model.Aggregates;

/// <summary>
/// Represents a payment transaction within the system.
/// Contains details about the amount, method, status, and associated booking.
/// </summary>
public partial class Payment
{
    public Payment()
    {
        TransactionId = Guid.NewGuid().ToString();
        PaymentMethod = string.Empty;
        AmountRecord = null!;
        Card = null!;
        Status = PaymentStatus.Pending;
        PaymentDate = DateTime.UtcNow;
    }

    public Payment(ProcessPaymentCommand command) : this()
    {
        BookingId = command.BookingId;
        AmountRecord = new Money(command.Amount);
        PaymentMethod = command.PaymentMethod; // e.g., "Credit Card"
        Card = new CreditCard(
            command.CardNumber,
            command.CardHolderName,
            command.ExpirationDate,
            command.Cvv);
    }

    /// <summary>
    /// The unique identifier of the payment.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// The identifier of the booking associated with this payment.
    /// </summary>
    public int BookingId { get; private set; }

    /// <summary>
    /// The unique transaction identifier generated for this payment.
    /// </summary>
    public string TransactionId { get; private set; } // UUID único de la transacción

    /// <summary>
    /// The monetary amount of the payment, encapsulated in a <see cref="Money"/> Value Object.
    /// </summary>
    public Money AmountRecord { get; private set; }

    /// <summary>
    /// The method used for the payment (e.g., Credit Card).
    /// </summary>
    public string PaymentMethod { get; private set; }

    /// <summary>
    /// The credit card information, encapsulated in a <see cref="CreditCard"/> Value Object.
    /// Contains the masked card number and card holder name.
    /// </summary>
    public CreditCard Card { get; private set; }

    /// <summary>
    /// The date when the payment was processed.
    /// </summary>
    public DateTime PaymentDate { get; private set; }

    /// <summary>
    /// The current status of the payment.
    /// </summary>
    public PaymentStatus Status { get; private set; }

    /// <summary>
    /// Marks the payment as successfully completed and emits a <see cref="PaymentCompletedEvent"/>.
    /// </summary>
    public void Complete()
    {
        Status = PaymentStatus.Completed;
        AddDomainEvent(new PaymentCompletedEvent(Id, BookingId));
    }

    /// <summary>
    /// Marks the payment as failed.
    /// </summary>
    public void Fail()
    {
        Status = PaymentStatus.Failed;
    }
}

/// <summary>
/// Enumeration of possible payment statuses.
/// </summary>
public enum PaymentStatus
{
    Pending = 0,
    Completed = 1,
    Failed = 2
}