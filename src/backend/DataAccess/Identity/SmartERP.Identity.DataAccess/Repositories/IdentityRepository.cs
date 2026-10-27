using Microsoft.AspNetCore.Identity;
using SmartERP.Identity.Domain.Entities;
using SmartERP.Identity.Repositories.Interfaces;
using SmartERP.Identity.Repositories.Models;

namespace SmartERP.Identity.DataAccess.Repositories;

public sealed class IdentityRepository : IIdentityRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    public IdentityRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<ApplicationUser?> FindByEmailAsync(string email,CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<ApplicationUser?> FindByIdAsync(int userId,CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _userManager.FindByIdAsync(userId.ToString());
    }

    public async Task<IdentityOperationResult> CreateUserAsync(ApplicationUser user,string password,CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await _userManager.CreateAsync(user, password);

        return result.Succeeded
            ? IdentityOperationResult.Success()
            : IdentityOperationResult.Failure(
                result.Errors.Select(error => error.Description));
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user,CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task<IdentityOperationResult> ConfirmEmailAsync(ApplicationUser user,string token,CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await _userManager.ConfirmEmailAsync(user, token);

        return result.Succeeded
            ? IdentityOperationResult.Success()
            : IdentityOperationResult.Failure(
                result.Errors.Select(error => error.Description));
    }


    public async Task<PasswordCheckResult> CheckPasswordAsync(ApplicationUser user,string password,CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            password,
            lockoutOnFailure: true);

        return new PasswordCheckResult
        {
            Succeeded = result.Succeeded,
            IsLockedOut = result.IsLockedOut,
            IsNotAllowed = result.IsNotAllowed
        };
    }
    public async Task<IReadOnlyCollection<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var roles = await _userManager.GetRolesAsync(user);
        return roles.ToArray();
    }

    public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<IdentityOperationResult> ResetPasswordAsync(
        ApplicationUser user,
        string token,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await _userManager.ResetPasswordAsync(
            user,
            token,
            newPassword);

        return ToOperationResult(result);
    }

    public async Task<ApplicationUser?> FindByExternalLoginAsync(
        string loginProvider,
        string providerKey,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _userManager.FindByLoginAsync(
            loginProvider,
            providerKey);
    }

    public async Task<IdentityOperationResult> CreateExternalUserAsync(
        ApplicationUser user,
        string loginProvider,
        string providerKey,
        string defaultRole,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var createResult = await _userManager.CreateAsync(user);

        if (!createResult.Succeeded)
        {
            return ToOperationResult(createResult);
        }

        var loginResult = await _userManager.AddLoginAsync(
            user,
            new UserLoginInfo(loginProvider, providerKey, loginProvider));

        if (!loginResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            return ToOperationResult(loginResult);
        }

        var roleResult = await _userManager.AddToRoleAsync(
            user,
            defaultRole);

        if (roleResult.Succeeded)
        {
            return IdentityOperationResult.Success();
        }

        await _userManager.DeleteAsync(user);
        return ToOperationResult(roleResult);
    }

    public async Task<IdentityOperationResult> AddExternalLoginAsync(
        ApplicationUser user,
        string loginProvider,
        string providerKey,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = await _userManager.AddLoginAsync(
            user,
            new UserLoginInfo(loginProvider, providerKey, loginProvider));

        return ToOperationResult(result);
    }

    public async Task<IdentityOperationResult> MarkEmailConfirmedAsync(
        ApplicationUser user,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        user.EmailConfirmed = true;
        user.UpdatedAtUtc = DateTimeOffset.UtcNow;

        return ToOperationResult(await _userManager.UpdateAsync(user));
    }

    private static IdentityOperationResult ToOperationResult(
        IdentityResult result)
    {
        return result.Succeeded
            ? IdentityOperationResult.Success()
            : IdentityOperationResult.Failure(
                result.Errors.Select(error => error.Description));
    }
}
