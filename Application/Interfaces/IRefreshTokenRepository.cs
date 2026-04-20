using DotnetApi.Domain.Entities;

namespace DotnetApi.Application.Interfaces;

public interface IRefreshTokenRepository
{
  Task<RefreshTokenEntity?> GetByTokenAsync(string token);
  Task AddAsync(RefreshTokenEntity refreshToken);
  Task UpdateAsync(RefreshTokenEntity refreshToken);
}