namespace SmartERP.Sales.API.Authentication;
public interface IRequestContext{int UserId{get;}int CompanyId{get;}}
public sealed class RequestContext(IHttpContextAccessor accessor):IRequestContext{public int UserId=>Read(accessor.HttpContext?.User.FindFirst("sub")?.Value,"authenticated user");public int CompanyId=>Read(accessor.HttpContext?.Request.Headers["X-Company-ID"].FirstOrDefault(),"company context");private static int Read(string? value,string name)=>int.TryParse(value,out var result)&&result>0?result:throw new UnauthorizedAccessException($"A valid {name} is required.");}
