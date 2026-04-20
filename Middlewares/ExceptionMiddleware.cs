using System.Net;
using System.Text.Json;
using DotnetApi.Application.Exceptions;

namespace DotnetApi.Middlewares;

public class ExceptionMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<ExceptionMiddleware> _logger;

  public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
  {
    _next = next;
    _logger = logger;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await _next(context);
    }
    catch (BadRequestException ex)
    {
      _logger.LogWarning(ex, "Bad request exception");

      context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
      context.Response.ContentType = "application/json";

      var response = new
      {
        message = ex.Message,
        traceId = context.TraceIdentifier
      };

      await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
    catch (Exception ex)
    {
      _logger.LogError(
        ex,
        "Unhandled exception | TraceId: {TraceId} | Path: {Path} | Method: {Method}",
        context.TraceIdentifier,
        context.Request.Path,
        context.Request.Method);

      context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
      context.Response.ContentType = "application/json";

      var response = new
      {
        message = "Something went wrong",
        traceId = context.TraceIdentifier
      };

      await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
  }
}