using BackendAwSmartstay.API.Payments.Domain.Model.Entities;
using BackendAwSmartstay.API.Payments.Domain.Model.ValueObjects;

namespace BackendAwSmartstay.API.Payments.Domain.Model.Aggregates;

/// <summary>
/// Represents a guest folio aggregate root that tracks charges and payments
/// for a booking. Uses <see cref="FolioPaymentRecord"/> (a snapshot Value Object)
/// instead of a direct reference to the <see cref="Payment"/> aggregate,
/// ensuring clean bounded-context isolation.
/// </summary>
public class GuestFolio
{
    private readonly List<Charge> _charges = new();
    private readonly List<FolioPaymentRecord> _paymentRecords = new();

    /// <summary>
    /// Gets the unique identifier of the folio.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Gets the identifier of the booking associated with this folio.
    /// </summary>
    public int BookingId { get; private set; }

    /// <summary>
    /// Gets the current status of the folio.
    /// </summary>
    public FolioStatus Status { get; private set; }

    /// <summary>
    /// Gets the read-only collection of charges applied to this folio.
    /// </summary>
    public IReadOnlyCollection<Charge> Charges => _charges.AsReadOnly();

    /// <summary>
    /// Gets the read-only collection of payment records applied to this folio.
    /// </summary>
    public IReadOnlyCollection<FolioPaymentRecord> PaymentRecords => _paymentRecords.AsReadOnly();

    /// <summary>
    /// Initializes a new instance of the <see cref="GuestFolio"/> class.
    /// </summary>
    /// <param name="bookingId">The booking identifier this folio belongs to.</param>
    public GuestFolio(int bookingId)
    {
        BookingId = bookingId;
        Status = FolioStatus.Open;
    }

    /// <summary>
    /// Private parameterless constructor for EF Core materialization.
    /// </summary>
    private GuestFolio()
    {
    }

    /// <summary>
    /// Adds a charge to the folio.
    /// </summary>
    /// <param name="category">The category of the charge.</param>
    /// <param name="description">A description of the charge.</param>
    /// <param name="amount">The monetary amount of the charge.</param>
    /// <exception cref="InvalidOperationException">Thrown when the folio is not open.</exception>
    public void AddCharge(ChargeCategory category, string description, Money amount)
    {
        if (Status != FolioStatus.Open)
            throw new InvalidOperationException($"Cannot add charges to a folio that is {Status}.");

        var charge = new Charge(category, description, amount, Id);
        _charges.Add(charge);
    }

    /// <summary>
    /// Registers a payment against this folio.
    /// Only the payment identifier and amount are stored as a snapshot to avoid coupling.
    /// </summary>
    /// <param name="paymentId">The identifier of the completed payment.</param>
    /// <param name="amount">The monetary amount of the payment.</param>
    /// <exception cref="InvalidOperationException">Thrown when the folio is not open.</exception>
    public void RegisterPayment(int paymentId, Money amount)
    {
        if (Status != FolioStatus.Open)
            throw new InvalidOperationException($"Cannot register payments on a folio that is {Status}.");

        var record = new FolioPaymentRecord(paymentId, amount);
        _paymentRecords.Add(record);
    }

    /// <summary>
    /// Calculates the current balance of the folio.
    /// Positive means the guest owes money; zero means the folio is settled.
    /// Returns a decimal instead of <see cref="Money"/> because a zero balance
    /// is a valid business state for a folio, while Money enforces Amount > 0.
    /// </summary>
    /// <returns>The current balance as a decimal value.</returns>
    public decimal CalculateBalance()
    {
        var totalCharges = _charges.Sum(c => c.AmountRecord.Amount);
        var totalPayments = _paymentRecords.Sum(p => p.Amount.Amount);
        return totalCharges - totalPayments;
    }

    /// <summary>
    /// Closes the folio if and only if the balance is zero.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the folio is not open or when there is an outstanding balance.
    /// </exception>
    public void CloseFolio()
    {
        if (Status != FolioStatus.Open)
            throw new InvalidOperationException($"Cannot close a folio that is {Status}.");

        var balance = CalculateBalance();
        if (balance != 0)
            throw new InvalidOperationException(
                $"Cannot close folio with outstanding balance of {balance:F2}. " +
                "All charges must be fully paid before closing.");

        Status = FolioStatus.Closed;
    }
}
