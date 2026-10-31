using SmartERP.Organization.Domain.Entities;
using SmartERP.Organization.Domain.Enums;
using SmartERP.Organization.Repositories.Interfaces;
using SmartERP.Organization.Services.DTO.Company;
using SmartERP.Organization.Services.Interfaces;

namespace SmartERP.Organization.Services.Implementations;

public sealed class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyService(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<CompanyResponseDto> CreateAsync(CreateCompanyRequestDto requestDto, int currentUserId, CancellationToken cancellationToken = default)
    {
        EnsureValidUserId(currentUserId);

        var normalizedCode =
            requestDto.Code.Trim().ToUpperInvariant();

        var codeIsUsed =
            await _companyRepository.IsCodeInUseAsync(
                normalizedCode,
                cancellationToken: cancellationToken);

        if (codeIsUsed)
        {
            throw new InvalidOperationException(
                "Company code is already in use.");
        }

        var utcNow = DateTimeOffset.UtcNow;

        var company = new Company
        {
            Name = requestDto.Name.Trim(),
            LegalName = requestDto.LegalName.Trim(),
            Code = normalizedCode,
            RegistrationNumber =
                NormalizeOptional(requestDto.RegistrationNumber),
            TaxNumber =
                NormalizeOptional(requestDto.TaxNumber),
            Email =
                NormalizeOptional(requestDto.Email)?.ToLowerInvariant(),
            Phone =
                NormalizeOptional(requestDto.Phone),
            IsActive = true,
            CreatedAtUtc = utcNow,
            CreatedByUserId = currentUserId
        };

        company.Memberships.Add(new CompanyMembership
        {
            UserId = currentUserId,
            Role = CompanyMembershipRole.Owner,
            IsActive = true,
            CreatedAtUtc = utcNow,
            CreatedByUserId = currentUserId
        });

        company.Settings = new CompanySettings
        {
            DefaultLanguage = "en",
            TimeZoneId = "Asia/Amman",
            DateFormat = "yyyy-MM-dd",
            TimeFormat = "HH:mm",
            NumberDecimalPlaces = 2,
            QuantityDecimalPlaces = 3,
            SalesInvoicePrefix = "INV",
            PurchaseOrderPrefix = "PO",
            CreatedAtUtc = utcNow,
            CreatedByUserId = currentUserId
        };

        await _companyRepository.AddAsync(
            company,
            cancellationToken);

        await _companyRepository.SaveChangesAsync(
            cancellationToken);

        return MapToResponse(company);
    }

    public async Task<CompanyResponseDto> GetByIdAsync(int companyId, int currentUserId, CancellationToken cancellationToken = default)
    {
        EnsureValidUserId(currentUserId);

        var company = await GetCompanyAsync(
            companyId,
            cancellationToken);

        await EnsureMembershipAsync(
            companyId,
            currentUserId,
            cancellationToken);

        return MapToResponse(company);
    }

    public async Task<IReadOnlyList<CompanyResponseDto>> GetForUserAsync(int currentUserId, CancellationToken cancellationToken = default)
    {
        EnsureValidUserId(currentUserId);

        var companies =
            await _companyRepository.GetForUserAsync(
                currentUserId,
                cancellationToken);

        return companies
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<CompanyResponseDto> UpdateAsync(int companyId, UpdateCompanyRequestDto requestDto, int currentUserId, CancellationToken cancellationToken = default)
    {
        EnsureValidUserId(currentUserId);

        var company = await GetCompanyAsync(
            companyId,
            cancellationToken);

        var membership = await EnsureMembershipAsync(
            companyId,
            currentUserId,
            cancellationToken);

        EnsureCanManageCompany(membership);

        company.Name = requestDto.Name.Trim();
        company.LegalName = requestDto.LegalName.Trim();
        company.RegistrationNumber =
            NormalizeOptional(requestDto.RegistrationNumber);
        company.TaxNumber =
            NormalizeOptional(requestDto.TaxNumber);
        company.Email =
            NormalizeOptional(requestDto.Email)?.ToLowerInvariant();
        company.Phone =
            NormalizeOptional(requestDto.Phone);
        company.UpdatedAtUtc = DateTimeOffset.UtcNow;
        company.UpdatedByUserId = currentUserId;

        await _companyRepository.SaveChangesAsync(
            cancellationToken);

        return MapToResponse(company);
    }

    public async Task ActivateAsync(int companyId, int currentUserId, CancellationToken cancellationToken = default)
    {
        var company = await GetCompanyAsync(
            companyId,
            cancellationToken);

        var membership = await EnsureMembershipAsync(
            companyId,
            currentUserId,
            cancellationToken);

        EnsureCanManageCompany(membership);

        company.IsActive = true;
        company.UpdatedAtUtc = DateTimeOffset.UtcNow;
        company.UpdatedByUserId = currentUserId;

        await _companyRepository.SaveChangesAsync(
            cancellationToken);
    }

    public async Task DeactivateAsync(int companyId, int currentUserId, CancellationToken cancellationToken = default)
    {
        var company = await GetCompanyAsync(
            companyId,
            cancellationToken);

        var membership = await EnsureMembershipAsync(
            companyId,
            currentUserId,
            cancellationToken);

        EnsureCanManageCompany(membership);

        company.IsActive = false;
        company.UpdatedAtUtc = DateTimeOffset.UtcNow;
        company.UpdatedByUserId = currentUserId;

        await _companyRepository.SaveChangesAsync(
            cancellationToken);
    }

    private async Task<Company> GetCompanyAsync(int companyId, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetByIdAsync(
            companyId,
            cancellationToken);

        return company
            ?? throw new KeyNotFoundException(
                "Company was not found.");
    }

    private async Task<CompanyMembership> EnsureMembershipAsync(int companyId, int currentUserId, CancellationToken cancellationToken)
    {
        var membership =
            await _companyRepository.GetMembershipAsync(
                companyId,
                currentUserId,
                cancellationToken);

        return membership
            ?? throw new UnauthorizedAccessException(
                "You do not have access to this company.");
    }

    private static void EnsureCanManageCompany(CompanyMembership membership)
    {
        if (membership.Role is not
            (CompanyMembershipRole.Owner or
             CompanyMembershipRole.Administrator))
        {
            throw new UnauthorizedAccessException(
                "You cannot manage this company.");
        }
    }

    private static void EnsureValidUserId(int userId)
    {
        if (userId <= 0)
        {
            throw new UnauthorizedAccessException(
                "The current user is invalid.");
        }
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private static CompanyResponseDto MapToResponse(Company company)
    {
        return new CompanyResponseDto(
            company.Id,
            company.Name,
            company.LegalName,
            company.Code,
            company.RegistrationNumber,
            company.TaxNumber,
            company.Email,
            company.Phone,
            company.IsActive,
            company.CreatedAtUtc,
            company.UpdatedAtUtc);
    }
}
