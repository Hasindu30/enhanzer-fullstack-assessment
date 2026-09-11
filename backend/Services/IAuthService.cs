using backend.DTOs;

namespace backend.Services;

public interface IAuthService
{
    Task<string> LoginAsync(LoginRequestDto request);
}
