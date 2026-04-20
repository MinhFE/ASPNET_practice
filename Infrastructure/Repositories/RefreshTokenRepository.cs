using DotnetApi.Domain.Entities;
using DotnetApi.Infrastructure.Data;
using DotnetApi.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
  private readonly AppDbContext _context;

  public RefreshTokenRepository(AppDbContext context)
  {
    _context = context;
  }

  public async Task<RefreshTokenEntity?> GetByTokenAsync(string token)
  {
    var tokenEntity = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
    return tokenEntity;
  }

  public async Task AddAsync(RefreshTokenEntity refreshToken)
  {
    _context.RefreshTokens.Add(refreshToken);
    await _context.SaveChangesAsync();
  }

  public async Task UpdateAsync(RefreshTokenEntity refreshToken)
  {
    _context.RefreshTokens.Update(refreshToken);
    await _context.SaveChangesAsync();
  }
}