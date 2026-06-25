namespace BackendAwSmartstay.API.Payments.Domain.Model.ValueObjects;

/// <summary>
/// Value Object that encapsulates monetary amounts with validation.
/// Ensures that an amount is always greater than zero.
/// </summary>
public sealed record Money
{
    /// <summary>
    /// Gets the monetary amount value.
    /// </summary>
    public decimal Amount { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Money"/> record.
    /// </summary>
    /// <param name="amount">The monetary amount. Must be greater than zero.</param>
    /// <exception cref="ArgumentException">Thrown when amount is less than or equal to zero.</exception>
    public Money(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("The amount must be greater than zero.", nameof(amount));

        Amount = amount;
    }

    /// <summary>
    /// Implicitly converts a <see cref="Money"/> to its decimal value.
    /// </summary>
    /// <param name="money">The Money value object.</param>
    public static implicit operator decimal(Money money) => money.Amount;

    /// <summary>
    /// Implicitly converts a decimal value to a <see cref="Money"/>.
    /// </summary>
    /// <param name="amount">The decimal amount.</param>
    public static implicit operator Money(decimal amount) => new(amount);

    /// <summary>
    /// Returns the string representation of the amount.
    /// </summary>
    public override string ToString() => Amount.ToString("F2");
}
