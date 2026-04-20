namespace DotnetApi.Domain.Entities;

public class ProductEntity
{
  public Guid Id { get; set; }
  public string Name { get; set; } = "";
  public string? Price { get; set; }
}