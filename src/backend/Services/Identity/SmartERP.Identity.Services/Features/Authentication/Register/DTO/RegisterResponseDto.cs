namespace SmartERP.Identity.Services.Features.Authentication.Register.DTO;

public sealed record RegisterResponseDto(int UserId,string Email,bool RequiresEmailConfirmation);
