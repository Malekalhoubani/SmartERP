namespace SmartERP.Organization.Services.DTO.Company;

public sealed record CreateCompanyRequestDto(
    string Name,
    string LegalName,
    string Code,
    string? RegistrationNumber,
    string? TaxNumber,
    string? Email,
    string? Phone);
