namespace BackendAwSmartstay.API.Payments.Domain.Model.ValueObjects;

/// <summary>
/// Value Object that encapsulates credit card information.
/// Validates the raw card number and computes a masked version for secure storage.
/// Only the holder name and masked number are persisted; raw sensitive data is discarded.
/// </summary>
public sealed record CreditCard
{
    /// <summary>
    /// Gets the card holder name.
    /// </summary>
    public string HolderName { get; init; }

    /// <summary>
    /// Gets the masked credit card number (e.g., "**** **** **** 1234").
    /// </summary>
    public string MaskedNumber { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CreditCard"/> record.
    /// Validates the raw card number and computes the masked representation.
    /// </summary>
    /// <param name="number">The raw credit card number. Must not be empty.</param>
    /// <param name="holderName">The card holder name.</param>
    /// <param name="expirationDate">The card expiration date (MM/YY). Used only for validation; not persisted.</param>
    /// <param name="cvv">The card CVV code. Used only for validation; not persisted.</param>
    /// <exception cref="ArgumentException">Thrown when the card number is null or empty.</exception>
    public CreditCard(string number, string holderName, string expirationDate, string cvv)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("Card number cannot be empty.", nameof(number));

        HolderName = holderName;
        MaskedNumber = number.Length > 4
            ? "**** **** **** " + number.Substring(number.Length - 4)
            : "****";
    }

    /// <summary>
    /// Private parameterless constructor for EF Core materialization.
    /// </summary>
    private CreditCard()
    {
        HolderName = string.Empty;
        MaskedNumber = string.Empty;
    }
}
