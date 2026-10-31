using SmartERP.Organization.API.Filters;
using SmartERP.Organization.Services.DTO.Company;
using SmartERP.Organization.Services.Interfaces;

namespace SmartERP.Organization.API.Endpoints;

public static class CompanyEndpoints
{
    public static IEndpointRouteBuilder MapCompanyEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var companyGroup = endpoints
            .MapGroup("/api/companies")
            .WithTags("Companies")
            .RequireAuthorization();

        companyGroup
            .MapPost("/", CreateAsync)
            .AddEndpointFilter<
                ValidationFilter<CreateCompanyRequestDto>>();

        companyGroup
            .MapGet("/", GetForCurrentUserAsync);

        companyGroup
            .MapGet("/{companyId:int}", GetByIdAsync);

        companyGroup
            .MapPut("/{companyId:int}", UpdateAsync)
            .AddEndpointFilter<
                ValidationFilter<UpdateCompanyRequestDto>>();

        companyGroup
            .MapPatch(
                "/{companyId:int}/activate",
                ActivateAsync);

        companyGroup
            .MapPatch(
                "/{companyId:int}/deactivate",
                DeactivateAsync);

        return endpoints;
    }

    private static async Task<IResult> CreateAsync(CreateCompanyRequestDto requestDto,ICompanyService companyService,ICurrentUserService currentUserService,CancellationToken cancellationToken)
    {
        var responseDto = await companyService.CreateAsync(
            requestDto,
            currentUserService.UserId,
            cancellationToken);

        return Results.Created(
            $"/api/companies/{responseDto.Id}",
            responseDto);
    }

    private static async Task<IResult> GetForCurrentUserAsync(ICompanyService companyService,ICurrentUserService currentUserService,CancellationToken cancellationToken)
    {
        var companies = await companyService.GetForUserAsync(
            currentUserService.UserId,
            cancellationToken);

        return Results.Ok(companies);
    }

    private static async Task<IResult> GetByIdAsync(int companyId,ICompanyService companyService,ICurrentUserService currentUserService,CancellationToken cancellationToken)
    {
        var company = await companyService.GetByIdAsync(
            companyId,
            currentUserService.UserId,
            cancellationToken);

        return Results.Ok(company);
    }

    private static async Task<IResult> UpdateAsync(int companyId,UpdateCompanyRequestDto requestDto,ICompanyService companyService,ICurrentUserService currentUserService,CancellationToken cancellationToken)
    {
        var company = await companyService.UpdateAsync(
            companyId,
            requestDto,
            currentUserService.UserId,
            cancellationToken);

        return Results.Ok(company);
    }

    private static async Task<IResult> ActivateAsync(int companyId,ICompanyService companyService,ICurrentUserService currentUserService,CancellationToken cancellationToken)
    {
        await companyService.ActivateAsync(
            companyId,
            currentUserService.UserId,
            cancellationToken);

        return Results.NoContent();
    }

    private static async Task<IResult> DeactivateAsync(int companyId,ICompanyService companyService,ICurrentUserService currentUserService,CancellationToken cancellationToken)
    {
        await companyService.DeactivateAsync(
            companyId,
            currentUserService.UserId,
            cancellationToken);

        return Results.NoContent();
    }
}