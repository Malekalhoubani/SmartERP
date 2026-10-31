using System.Net;
using System.Net.Http.Headers;

namespace SmartERP.Inventory.API.Authentication;

public interface ICompanyAccessValidator
{
    Task<bool> HasAccessAsync(int companyId,CancellationToken cancellationToken = default);
}

public sealed class CompanyAccessValidator(IHttpClientFactory httpClientFactory,IHttpContextAccessor accessor) : ICompanyAccessValidator
{
    public async Task<bool> HasAccessAsync(int companyId,CancellationToken cancellationToken = default)
    {
        var authorization = accessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(authorization)) return false;
        using var request = new HttpRequestMessage(HttpMethod.Get,$"api/companies/{companyId}");
        request.Headers.Authorization = AuthenticationHeaderValue.Parse(authorization);
        using var response = await httpClientFactory.CreateClient("OrganizationAccess").SendAsync(request,cancellationToken);
        return response.StatusCode == HttpStatusCode.OK;
    }
}

public sealed class CompanyAccessFilter(ICompanyAccessValidator validator,IRequestContext context) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext invocation,EndpointFilterDelegate next)
    {
        return await validator.HasAccessAsync(context.CompanyId,invocation.HttpContext.RequestAborted)
            ? await next(invocation)
            : Results.Forbid();
    }
}
