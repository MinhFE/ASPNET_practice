using DotnetApi.Domain.Entities;

namespace DotnetApi.Application.Interfaces;

public interface IProductRepository
{
  IQueryable<ProductEntity> Query();
  void Add(ProductEntity product);
}