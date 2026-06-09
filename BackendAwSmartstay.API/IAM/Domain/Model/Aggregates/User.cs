using System.Text.Json.Serialization;
using BackendAwSmartstay.API.IAM.Domain.Model.Constants;
using BackendAwSmartstay.API.IAM.Domain.Model.Enums;

namespace BackendAwSmartstay.API.IAM.Domain.Model.Aggregates;

/// <summary>
/// User Aggregate Root.
/// Represents a registered user within the identity context.
/// </summary>
public class User
{
    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class.
    /// </summary>
    public User(string username, string passwordHash, string role,
        UserStatus status = UserStatus.Active,
        int? hotelId = null,
        int? chainId = null,
        int tokenVersion = 0)
    {
        Username = username;
        PasswordHash = passwordHash;
        Role = role;
        Status = status;
        HotelId = hotelId;
        ChainId = chainId;
        TokenVersion = tokenVersion;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Required by Entity Framework Core for materialization.
    /// Protected to enforce the use of the parameterized constructor in application code.
    /// </summary>
    public User()
    {
        Username = string.Empty;
        PasswordHash = string.Empty;
        Role = UserRoles.Guest;
        Status = UserStatus.Active;
        TokenVersion = 0;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public int Id { get; private set; }
    public string Username { get; private set; }
    [JsonIgnore] public string PasswordHash { get; private set; }
    public string Role { get; private set; }
    public UserStatus Status { get; private set; }
    public int? HotelId { get; private set; }
    public int? ChainId { get; private set; }
    public int TokenVersion { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public User UpdateUsername(string username)
    {
        Username = username;
        return this;
    }

    public User UpdatePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty.", nameof(passwordHash));
        PasswordHash = passwordHash;
        return this;
    }
}