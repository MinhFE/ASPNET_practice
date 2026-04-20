using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DotnetApi.Application.Dtos;
using DotnetApi.Application.Interfaces;
using DotnetApi.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace DotnetApi.Application.Services;

public class AuthService : IAuthService
{
  private readonly IConfiguration _config;
  private readonly IRefreshTokenRepository _refreshTokenRepository;

  public AuthService(IConfiguration config, IRefreshTokenRepository refreshTokenRepository)
  {
    _config = config;
    _refreshTokenRepository = refreshTokenRepository;
  }

  public async Task<AuthResponse> LoginAsync(string email, string roles)
  {
    var token = GenerateToken(email, roles);
    var refreshToken = GenerateRefreshToken();

    var fakeUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    var refreshTokenEntity = new RefreshTokenEntity
    {
      Id = Guid.NewGuid(),
      UserId = fakeUserId, // This should be the actual user ID from your database
      Token = refreshToken,
      ExpiryDate = DateTime.UtcNow.AddDays(7),
      IsRevoked = false
    };

    await _refreshTokenRepository.AddAsync(refreshTokenEntity);

    return new AuthResponse
    {
      AccessToken = token,
      RefreshToken = refreshToken
    };
  }

  public string GenerateRefreshToken()
  {
    return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
  }

  public string GenerateToken(string email, string roles)
  {
    var claims = new[]
    {
      new Claim(ClaimTypes.Email, email),
      new Claim(ClaimTypes.Role, roles),
      new Claim("permission", "update_product")
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
      issuer: _config["Jwt:Issuer"],
      audience: _config["Jwt:Audience"],
      claims: claims,
      expires: DateTime.UtcNow.AddMinutes(15),
      signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }

  public async Task<AuthResponse> RefreshToken(string refreshToken, string email, string roles)
  {
    var refreshTokenEntity = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

    if (refreshTokenEntity == null || refreshTokenEntity.IsRevoked || refreshTokenEntity.ExpiryDate < DateTime.UtcNow)
    {
      throw new UnauthorizedAccessException("Invalid refresh token");
    }

    // lấy user thật từ DB theo refreshTokenEntity.UserId
    // var user = await _userRepository.GetByIdAsync(refreshTokenEntity.UserId);
    var newToken = GenerateToken(email, roles);
    var newRefreshToken = GenerateRefreshToken();


    refreshTokenEntity.IsRevoked = true;
    await _refreshTokenRepository.UpdateAsync(refreshTokenEntity);

    var newRefreshTokenEntity = new RefreshTokenEntity
    {
      Id = Guid.NewGuid(),
      UserId = refreshTokenEntity.UserId,
      Token = newRefreshToken,
      ExpiryDate = DateTime.UtcNow.AddDays(7),
      IsRevoked = false
    };

    await _refreshTokenRepository.AddAsync(newRefreshTokenEntity);

    return new AuthResponse
    {
      AccessToken = newToken,
      RefreshToken = newRefreshToken
    };
  }

  public async Task Logout(string refreshToken)
  {
    var refreshTokenEntity = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

    if (refreshTokenEntity != null)
    {
      refreshTokenEntity.IsRevoked = true;
      await _refreshTokenRepository.UpdateAsync(refreshTokenEntity);
    }
  }
}