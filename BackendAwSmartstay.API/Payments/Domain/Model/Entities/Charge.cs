using BackendAwSmartstay.API.Payments.Domain.Model.ValueObjects;

namespace BackendAwSmartstay.API.Payments.Domain.Model.Entities;

/// <summary>
/// Represents an immutable charge applied to a guest folio.
/// Once created, its values cannot be modified.
/// </summary>
public class Charge
{
    /// <summary>
    /// Gets the unique identifier of the charge.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Gets the category of the charge (e.g., Room, Restaurant, Spa).
    /// </summary>
    public ChargeCategory Category { get; private set; }

    /// <summary>
    /// Gets a human-readable description of the charge.
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Gets the monetary amount of the charge.
    /// </summary>
    public Money AmountRecord { get; private set; }

    /// <summary>
    /// Gets the date and time when the charge was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the identifier of the folio this charge belongs to.
    /// </summary>
    public int FolioId { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Charge"/> class.
    /// </summary>
    /// <param name="category">The category of the charge.</param>
    /// <param name="description">A description of the charge.</param>
    /// <param name="amount">The monetary amount.</param>
    /// <param name="folioId">The identifier of the parent folio.</param>
    public Charge(ChargeCategory category, string description, Money amount, int folioId)
    {
        Category = category;
        Description = description;
        AmountRecord = amount;
        CreatedAt = DateTime.UtcNow;
        FolioId = folioId;
    }

    /// <summary>
    /// Private parameterless constructor for EF Core materialization.
    /// </summary>
    private Charge()
    {
        Description = string.Empty;
        AmountRecord = null!;
    }
}
