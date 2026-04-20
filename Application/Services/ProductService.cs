using DotnetApi.Application.Dtos;
using DotnetApi.Domain.Entities;
using DotnetApi.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi.Application.Services;

public class ProductService : IProductService
{
  private readonly IProductRepository _productRepository;
  private readonly ILogger<ProductService> _logger;
  private readonly ICacheService _cacheService;
  public ProductService(IProductRepository productRepository, ILogger<ProductService> logger, ICacheService cacheService)
  {
    _productRepository = productRepository;
    _logger = logger;
    _cacheService = cacheService;
  }
  public async Task<PageResult<ProductResponse>> GetProductsAsync(GetProductsRequest request)
  {
    var cacheKey = $"products:{request.Page}:{request.PageSize}:{request.Keyword}";
    var cachedData = await _cacheService.GetAsync<PageResult<ProductResponse>>(cacheKey);
    if (cachedData != null)
    {
      _logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
      return cachedData;
    }

    var query = _productRepository.Query();

    var totalItems = await query.CountAsync();
    var products = await query
    .Where(x => x.Name.Contains(request.Keyword ?? ""))
    .Skip((request.Page - 1) * request.PageSize)
    .Take(request.PageSize)
    .ToListAsync();

    var result = new PageResult<ProductResponse>
    {
      Data = products.Select(p => new ProductResponse
      {
        Id = p.Id,
        Name = p.Name,
        Price = p.Price
      }).ToList(),
      TotalCount = totalItems,
      Page = request.Page,
      PageSize = request.PageSize
    };

    await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));
    return result;
  }

  public async Task<ProductResponse> AddProduct(CreateProductRequest product)
  {
    var entity = new ProductEntity
    {
      Id = Guid.NewGuid(),
      Name = product.Name,
      Price = product?.Price
    };

    await _cacheService.RemoveAsync("products:*");

    _productRepository.Add(entity);
    return new ProductResponse
    {
      Id = entity.Id,
      Name = entity.Name,
      Price = entity.Price
    };
  }
}