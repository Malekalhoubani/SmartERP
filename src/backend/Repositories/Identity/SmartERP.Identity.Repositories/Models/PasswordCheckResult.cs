namespace SmartERP.Identity.Repositories.Models;

public sealed class PasswordCheckResult
{
    public bool Succeeded { get; init; }

    public bool IsLockedOut { get; init; }

    public bool IsNotAllowed { get; init; }
}
