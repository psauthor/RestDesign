using System.Text.Json;
using System.Threading;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace RestDesign.Services;

public class RestDesignExceptionHandler : IExceptionHandler
{
  private readonly IHostEnvironment _environment;

  public RestDesignExceptionHandler(IHostEnvironment environment)
  {
    _environment = environment;
  }

  public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,
      Exception exception,
      CancellationToken cancellationToken)
  {
    if (exception is not null)
    {
      httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
      httpContext.Response.ContentType = "application/json";

      // Generate the response
      object? error;

      if (_environment.IsProduction())
      {
        error = new
        {
          Message = "Server Error. Please contact support.",
          MessageKey = "exceptionThrownServer",
          StackTrace = exception.StackTrace,
          ExceptionType = exception.GetType().Name
        };
      }
      else
      {
        error = new
        {
          Message = exception.Message,
          MessageKey = "",
          StackTrace = exception.StackTrace,
          ExceptionType = exception.GetType().Name,
          InnerException = exception.InnerException?.ToString()
        };
      }

      await httpContext.Response.WriteAsJsonAsync(error);

      return true;
    }

    return false;
  }
}
