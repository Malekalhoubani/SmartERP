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
}
