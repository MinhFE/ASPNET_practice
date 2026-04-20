using System.Text;
using DotnetApi.Infrastructure.Data;
using DotnetApi.Middlewares;
using DotnetApi.Infrastructure.Repositories;
using DotnetApi.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using DotnetApi.Application.Interfaces;
using StackExchange.Redis;
using DotnetApi.Infrastructure.Caching;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
  options.UseSqlite(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddSingleton<IConnectionMultiplexer>(
  ConnectionMultiplexer.Connect("localhost:6379")
);

builder.Services.AddControllers();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<ICacheService, RedisCacheService>();

builder.Services.Configure<ApiBehaviorOptions>(option =>
{
  option.InvalidModelStateResponseFactory = context =>
  {
    var errors = context.ModelState
    .Where(x => x.Value?.Errors.Count > 0)
    .ToDictionary(
      x => x.Key,
      x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray());
    return new BadRequestObjectResult(new
    {
      message = "Validation failed",
      errors
    });
  };
});

var jwtConfig = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,

    ValidIssuer = jwtConfig["Issuer"],
    ValidAudience = jwtConfig["Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"]!))
  };
});

builder.Services.AddAuthorization(options =>
{
  options.AddPolicy("CreateProductPolicy", policy =>
  {
    policy.RequireClaim("permission", "create_product");
  });

  options.AddPolicy("AdminOnly", policy =>
  {
    policy.RequireAssertion(context =>
    {
      var isAdmin = context.User.IsInRole("Admin");
      var isCreateProduct = context.User.HasClaim("permission", "create_product");
      return isAdmin || isCreateProduct;
    });
  });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
  var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
  db.Database.Migrate();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
