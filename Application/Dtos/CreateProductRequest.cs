using System.ComponentModel.DataAnnotations;

namespace DotnetApi.Application.Dtos;

public class CreateProductRequest
{
  [Required(ErrorMessage = "Name is required")]
  [MinLength(3, ErrorMessage = "Name must be at least 3 characters long")]
  public string Name { get; init; } = "";
  public string? Price { get; init; }
}