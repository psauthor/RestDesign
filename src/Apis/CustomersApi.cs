using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using WilderMinds.MinimalApiDiscovery;

namespace DesigningApis.Apis;

public class CustomersApi : IApi
{
  public void Register(IEndpointRouteBuilder builder)
  {
    var version = builder.NewVersionedApi();

    var group = version.MapGroup("/api/customers")
      .HasApiVersion(new ApiVersion(2, 0))
      .HasDeprecatedApiVersion(new ApiVersion(1, 0));

    group.MapGet("", GetAllCustomers);

  }

  private async Task<IResult> GetAllCustomers(BillingContext ctx)
  {
    var result = await ctx.Customers
      .OrderBy(c => c.CompanyName)
      .ToListAsync();

    return Results.Ok(result);
  }
}
