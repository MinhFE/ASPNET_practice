using DotnetApi.Application.Dtos;
using DotnetApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
  private readonly IProductService _productService;
  public ProductsController(IProductService productService)
  {
    _productService = productService;
  }

  [HttpGet]
  public async Task<IActionResult> GetProductsAsync([FromQuery] GetProductsRequest request)
  {
    var products = await _productService.GetProductsAsync(request);
    return Ok(products);
  }

  [Authorize(Policy = "AdminOnly")]
  [HttpPost]
  public IActionResult AddProduct([FromBody] CreateProductRequest product)
  {
    var addedProduct = _productService.AddProduct(product);
    return Ok(addedProduct);
  }
}