using SmartERP.Organization.Services.DTO.Company;

namespace SmartERP.Organization.Services.Interfaces;

public interface ICompanyService
{
    Task<CompanyResponseDto> CreateAsync(
        CreateCompanyRequestDto requestDto,
        int currentUserId,
        CancellationToken cancellationToken = default);

    Task<CompanyResponseDto> GetByIdAsync(
        int companyId,
        int currentUserId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CompanyResponseDto>> GetForUserAsync(
        int currentUserId,
        CancellationToken cancellationToken = default);

    Task<CompanyResponseDto> UpdateAsync(
        int companyId,
        UpdateCompanyRequestDto requestDto,
        int currentUserId,
        CancellationToken cancellationToken = default);

    Task ActivateAsync(
        int companyId,
        int currentUserId,
        CancellationToken cancellationToken = default);

    Task DeactivateAsync(
        int companyId,
        int currentUserId,
        CancellationToken cancellationToken = default);
}
