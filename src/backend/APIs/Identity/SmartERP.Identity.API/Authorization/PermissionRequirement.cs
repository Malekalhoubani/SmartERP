using Microsoft.AspNetCore.Authorization;

namespace SmartERP.Identity.API.Authorization;

public sealed record PermissionRequirement(string Permission)
    : IAuthorizationRequirement;
