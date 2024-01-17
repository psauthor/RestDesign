using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using WilderMinds.MinimalApiDiscovery;

namespace RestDesign.Apis;

public class DataApi : IApi
{
  public void Register(IEndpointRouteBuilder builder)
  {
    var group = builder.MapGroup("/api/data");

    group.MapMethods("dumpchanges", new [] { "OPTIONS" }, DumpChanges);
    group.MapGet("flaky", Flaky);
  }

  [HttpCacheExpiration(NoStore = true, MaxAge = 1)]
  public static IResult Flaky(HttpContext context)
  {
    var rando = Random.Shared.Next(2);
    if (rando == 1)
    {
      return Results.Ok("Worked This Time");
    }
    else
    {
      return Results.Problem("Failed to run");
    }
  }

  [HttpCacheExpiration(NoStore = true, MaxAge = 1)]
  public static async Task<IResult> DumpChanges(BillingContext context)
  {
    try
    {
      await context.ClearDatabaseAsync();
      return Results.Ok();
    }
    catch
    {
    }

    return Results.Problem("Could not dump the changes");
  }
}
