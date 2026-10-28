namespace SmartERP.Identity.Services.Features.Authentication.Login.Exceptions;

public sealed class LoginFailureException : Exception
{
    public LoginFailureException(string code, string message)
        : base(message)
    {
        Code = code;
    }

    public string Code { get; }
}
