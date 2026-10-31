namespace SmartERP.Organization.Services.DTO.Company;

public sealed record CompanyResponseDto(
    int Id,
    string Name,
    string LegalName,
    string Code,
    string? RegistrationNumber,
    string? TaxNumber,
    string? Email,
    string? Phone,
    bool IsActive,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc);