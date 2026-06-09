using BackendAwSmartstay.API.IAM.Application.OutboundServices;
using BackendAwSmartstay.API.IAM.Domain.Model.Aggregates;
using BackendAwSmartstay.API.IAM.Domain.Model.Commands;
using BackendAwSmartstay.API.IAM.Domain.Model.Constants;
using BackendAwSmartstay.API.IAM.Domain.Model.Exceptions;
using BackendAwSmartstay.API.IAM.Domain.Repositories;
using BackendAwSmartstay.API.IAM.Domain.Services;
using BackendAwSmartstay.API.Shared.Domain.Repositories;

namespace BackendAwSmartstay.API.IAM.Application.Internal.CommandServices;

/// <summary>
/// Service responsible for handling user-related commands (Write operations).
/// </summary>
public class UserCommandService(
    IUserRepository userRepository,
    ITokenService tokenService,
    IHashingService hashingService,
    IUnitOfWork unitOfWork) : IUserCommandService
{
    /// <summary>
    /// Processes a sign-in request.
    /// </summary>
    public async Task<(User user, string token)> Handle(SignInCommand command)
    {
        var user = await userRepository.FindByUsernameAsync(command.Username);
        if (user == null || !hashingService.VerifyPassword(command.Password, user.PasswordHash))
        {
            throw new InvalidCredentialsException();
        }
        var token = tokenService.GenerateToken(user);
        return (user, token);
    }

    /// <summary>
    /// Processes a sign-up request.
    /// </summary>
    public async Task Handle(SignUpCommand command)
    {
        if (await userRepository.ExistsByUsernameAsync(command.Username))
            throw new UsernameAlreadyExistsException(command.Username);

        var hashedPassword = hashingService.HashPassword(command.Password);
        var user = new User(command.Username, hashedPassword, UserRoles.Guest);

        await userRepository.AddAsync(user);
        await unitOfWork.CompleteAsync();
    }

    /// <summary>
    /// Processes a password change request.
    /// </summary>
    public async Task Handle(ChangePasswordCommand command)
    {
        var user = await userRepository.FindByIdAsync(command.UserId);
        if (user == null)
            throw new UserNotFoundException(command.UserId);
        if (!hashingService.VerifyPassword(command.CurrentPassword, user.PasswordHash))
            throw new InvalidCredentialsException();

        var newHashedPassword = hashingService.HashPassword(command.NewPassword);
        user.UpdatePasswordHash(newHashedPassword);

        await unitOfWork.CompleteAsync();
    }
}