namespace BackendAwSmartstay.API.Payments.Domain.Model.ValueObjects;

/// <summary>
/// Defines the possible states of a guest folio.
/// </summary>
public enum FolioStatus
{
    /// <summary>
    /// The folio is open and accepting charges and payments.
    /// </summary>
    Open = 0,

    /// <summary>
    /// The folio is closed. All charges have been paid and the balance is zero.
    /// </summary>
    Closed = 1,

    /// <summary>
    /// The folio has been cancelled.
    /// </summary>
    Cancelled = 2
}
