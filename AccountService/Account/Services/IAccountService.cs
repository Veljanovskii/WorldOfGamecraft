using AccountService.Models.DataTransferObjects;

namespace AccountService.Services;

public interface IAccountService
{
    Task<ServiceResult> RegisterUserAsync(RegisterDto registerDto);
    Task<string> LoginAsync(LoginDto loginDto);
}
