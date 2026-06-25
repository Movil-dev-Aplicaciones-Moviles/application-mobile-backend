namespace BackendAwSmartstay.API.Payments.Domain.Model.ValueObjects;

/// <summary>
/// Defines the origin categories for charges applied to a guest folio.
/// </summary>
public enum ChargeCategory
{
    /// <summary>
    /// Charge originating from room booking or accommodation services.
    /// </summary>
    Room = 0,

    /// <summary>
    /// Charge originating from restaurant or food and beverage services.
    /// </summary>
    Restaurant = 1,

    /// <summary>
    /// Charge originating from spa and wellness services.
    /// </summary>
    Spa = 2,

    /// <summary>
    /// Charge originating from property damage or incident.
    /// </summary>
    Damage = 3,

    /// <summary>
    /// Charge originating from laundry services.
    /// </summary>
    Laundry = 4
}
