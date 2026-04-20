using DotnetApi.Application.Interfaces;
using DotnetApi.Infrastructure.Data;
using DotnetApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
  private readonly AppDbContext _context;
  public ProductRepository(AppDbContext context)
  {
    _context = context;
  }

  public IQueryable<ProductEntity> Query()
  {
    return _context.Products.AsQueryable();
  }

  public void Add(ProductEntity product)
  {
    _context.Products.Add(product);
    _context.SaveChanges();
  }
}