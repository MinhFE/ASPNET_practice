using DotnetApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi.Infrastructure.Data;

public class AppDbContext : DbContext
{
  public DbSet<ProductEntity> Products => Set<ProductEntity>();
  public DbSet<RefreshTokenEntity> RefreshTokens => Set<RefreshTokenEntity>();
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
  {
  }

}