using SmartERP.Inventory.API.Authentication;
using SmartERP.Inventory.Domain.Enums;
using SmartERP.Inventory.Services.DTO;
using SmartERP.Inventory.Services.Interfaces;

namespace SmartERP.Inventory.API.Endpoints;

public static class CatalogEndpoints
{
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/catalog").RequireAuthorization().AddEndpointFilter<CompanyAccessFilter>().WithTags("Product Catalog");
        group.MapGet("/{type}",ListAsync);
        group.MapGet("/{type}/{itemId:int}",GetAsync);
        group.MapPost("/{type}",CreateAsync);
        group.MapPut("/{type}/{itemId:int}",UpdateAsync);
        group.MapPatch("/{type}/{itemId:int}/activate",(string type,int itemId,ICatalogService service,IRequestContext context,CancellationToken token) => SetStatusAsync(type,itemId,true,service,context,token));
        group.MapPatch("/{type}/{itemId:int}/deactivate",(string type,int itemId,ICatalogService service,IRequestContext context,CancellationToken token) => SetStatusAsync(type,itemId,false,service,context,token));
        return endpoints;
    }

    private static async Task<IResult> ListAsync(string type,ICatalogService service,IRequestContext context,CancellationToken token)
    {
        if (!TryType(type,out var catalogType)) return InvalidType();
        return Results.Ok(await service.ListAsync(context.CompanyId,catalogType,token));
    }

    private static async Task<IResult> GetAsync(string type,int itemId,ICatalogService service,IRequestContext context,CancellationToken token)
    {
        if (!TryType(type,out _)) return InvalidType();
        return Results.Ok(await service.GetAsync(context.CompanyId,itemId,token));
    }

    private static async Task<IResult> CreateAsync(string type,UpsertCatalogItemRequestDto request,ICatalogService service,IRequestContext context,CancellationToken token)
    {
        if (!TryType(type,out var catalogType)) return InvalidType();
        var result = await service.CreateAsync(context.CompanyId,context.UserId,catalogType,request,token);
        return Results.Created($"/api/catalog/{type}/{result.Id}",result);
    }

    private static async Task<IResult> UpdateAsync(string type,int itemId,UpsertCatalogItemRequestDto request,ICatalogService service,IRequestContext context,CancellationToken token)
    {
        if (!TryType(type,out _)) return InvalidType();
        return Results.Ok(await service.UpdateAsync(context.CompanyId,context.UserId,itemId,request,token));
    }

    private static async Task<IResult> SetStatusAsync(string type,int itemId,bool isActive,ICatalogService service,IRequestContext context,CancellationToken token)
    {
        if (!TryType(type,out _)) return InvalidType();
        await service.SetStatusAsync(context.CompanyId,context.UserId,itemId,isActive,token);
        return Results.NoContent();
    }

    private static bool TryType(string value,out CatalogItemType type)
    {
        type = value.ToLowerInvariant() switch
        {
            "products" => CatalogItemType.Product,
            "categories" => CatalogItemType.Category,
            "units-of-measure" => CatalogItemType.UnitOfMeasure,
            "brands" => CatalogItemType.Brand,
            "barcodes" => CatalogItemType.Barcode,
            "product-variants" => CatalogItemType.ProductVariant,
            "pricing" => CatalogItemType.Price,
            "tax-configuration" => CatalogItemType.TaxConfiguration,
            _ => 0
        };
        return Enum.IsDefined(type);
    }

    private static IResult InvalidType() => Results.ValidationProblem(new Dictionary<string,string[]> { ["type"] = ["Unsupported catalog type."] });
}
