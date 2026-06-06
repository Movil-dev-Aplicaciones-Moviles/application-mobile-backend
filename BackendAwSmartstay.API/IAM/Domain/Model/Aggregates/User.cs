using System.Text.Json.Serialization;
using BackendAwSmartstay.API.IAM.Domain.Model.Constants;

namespace BackendAwSmartstay.API.IAM.Domain.Model.Aggregates;

/// <summary>
///     User Aggregate Root.
///     Represents a registered user within the identity context.
/// </summary>
/// <remarks>
///     This class encapsulates user credentials and role information.
/// </remarks>
public class User
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="User"/> class.
    /// </summary>
    /// <param name="username">The unique username.</param>
    /// <param name="passwordHash">The bcrypt hashed password.</param>
    /// <param name="role">The user's role (e.g., 'guest', 'staff').</param>
    public User(string username, string passwordHash, string role)
    {
        Username = username;
        PasswordHash = passwordHash;
        Role = role;
    }
    /// <summary>
    ///     Required by Entity Framework Core for materialization.
    ///     Protected to enforce the use of the parameterized constructor in application code.
    /// </summary>
    public User() 
    {
        Username = string.Empty;
        PasswordHash = string.Empty;
        Role = UserRoles.Guest;
    }

    /// <summary>
    ///     Gets the unique identifier of the user.
    /// </summary>
    public int Id { get; private set; }
    
    /// <summary>
    ///     Gets the username.
    /// </summary>
    public string Username { get; private set; }

    /// <summary>
    ///     Gets the security password hash.
    /// </summary>
    [JsonIgnore] 
    public string PasswordHash { get; private set; }

    /// <summary>
    ///     Gets the assigned role of the user.
    /// </summary>
    public string Role { get; private set; }

    /// <summary>
    ///     Updates the username.
    /// </summary>
    /// <param name="username">The new username.</param>
    /// <returns>The updated user instance.</returns>
    public User UpdateUsername(string username)
    {
        Username = username;
        return this;
    }

    /// <summary>
    ///     Updates the password hash.
    /// </summary>
    /// <param name="passwordHash">The new password hash.</param>
    /// <returns>The updated user instance.</returns>
    public User UpdatePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty.", nameof(passwordHash));

        PasswordHash = passwordHash;
        return this;
    }
}