namespace BackendAwSmartstay.API.Payments.Domain.Model.ValueObjects;

/// <summary>
/// Value Object that represents a snapshot/projection of a payment applied to a folio.
/// This avoids coupling the GuestFolio aggregate root to the Payment aggregate root.
/// </summary>
/// <param name="PaymentId">The unique identifier of the payment.</param>
/// <param name="Amount">The monetary amount of the payment.</param>
public record FolioPaymentRecord(int PaymentId, Money Amount);
