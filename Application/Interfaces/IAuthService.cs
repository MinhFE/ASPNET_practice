using DotnetApi.Application.Dtos;

namespace DotnetApi.Application.Interfaces;

public interface IAuthService
{
  Task<AuthResponse> LoginAsync(string email, string roles);
  Task<AuthResponse> RefreshToken(string refreshToken, string email, string roles);
  Task Logout(string refreshToken);
}