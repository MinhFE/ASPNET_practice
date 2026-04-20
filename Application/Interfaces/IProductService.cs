using DotnetApi.Application.Dtos;

namespace DotnetApi.Application.Interfaces;
public interface IProductService
{
  Task<PageResult<ProductResponse>> GetProductsAsync(GetProductsRequest request);
  Task<ProductResponse> AddProduct(CreateProductRequest product);
}