namespace SmartERP.Identity.Repositories.Models;

public sealed class IdentityOperationResult
{
    public bool Succeeded { get; }

    public IReadOnlyCollection<string> Errors { get; }

    private IdentityOperationResult(bool succeeded,IReadOnlyCollection<string> errors)
    {
        Succeeded = succeeded;
        Errors = errors;
    }

    public static IdentityOperationResult Success()
    {
        return new IdentityOperationResult(true,Array.Empty<string>());
    }

    public static IdentityOperationResult Failure(IEnumerable<string> errors)
    {
        return new IdentityOperationResult(false,errors.ToArray());
    }
}