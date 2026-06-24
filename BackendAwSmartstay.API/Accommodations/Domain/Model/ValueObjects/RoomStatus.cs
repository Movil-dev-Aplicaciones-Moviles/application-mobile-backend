namespace BackendAwSmartstay.API.Accommodations.Domain.Model.ValueObjects;

/// <summary>
/// Represents the operational status of a room.
/// This strongly-typed enumeration eliminates primitive obsession
/// and provides compile-time safety for room status values.
/// </summary>
public enum RoomStatus
{
    /// <summary>
    /// The room is clean and ready for guests.
    /// </summary>
    Clean,
    
    /// <summary>
    /// The room is dirty and needs cleaning.
    /// </summary>
    Dirty,
    
    /// <summary>
    /// The room is under maintenance.
    /// </summary>
    Maintenance,
    
    /// <summary>
    /// The room is currently reserved by a guest.
    /// </summary>
    Reserved
}
