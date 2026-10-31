namespace SmartERP.Organization.Services.DTO.Company;

public sealed record UpdateCompanyRequestDto(
    string Name,
    string LegalName,
    string? RegistrationNumber,
    string? TaxNumber,
    string? Email,
    string? Phone);
