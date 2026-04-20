namespace DotnetApi.Application.Dtos;

public class ProductResponse
{
  public Guid Id { get; init; }
  public required string Name { get; init; }
  public string? Price { get; init; }
}