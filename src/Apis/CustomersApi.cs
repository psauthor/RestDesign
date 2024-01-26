using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using Swashbuckle.AspNetCore.Annotations;
using WilderMinds.MinimalApiDiscovery;

namespace RestDesign.Apis;

public class CustomersApi : IApi
{
  public void Register(IEndpointRouteBuilder builder)
  {
    var version = builder.NewVersionedApi();

    var group = version.MapGroup("/api/customers")
      .HasApiVersion(new ApiVersion(2, 0))
      .HasDeprecatedApiVersion(new ApiVersion(1, 0))
      .AddFluentValidationAutoValidation()
      .WithMetadata(
        new SwaggerOperationAttribute(
          summary: "Customers",
          description: "Customers that we can apply projects and tickets."));


    group.MapGet("", GetAll)
      .Produces<List<Customer>>(200, "application/json", "text/xml");
    group.MapGet("{id:int}", GetOne).WithName("GetOneCustomer");
    group.MapPost("", Post);
    group.MapPut("{id:int}", Update);
    group.MapDelete("{id:int}", Delete);

  }

  // Get All
  public static async Task<IResult> GetAll(BillingContext ctx, bool includeProjects = false)
  {
    IQueryable<Customer> query = includeProjects ? ctx.Customers.Include(c => c.Projects) : ctx.Customers;

    var result = await query
        .OrderBy(c => c.CompanyName)
        .ToListAsync();

    return Results.Ok(result);
  }

  // Get One
  public static async Task<IResult> GetOne(BillingContext ctx, int id, bool includeProjects = false)
  {
    IQueryable<Customer> query = includeProjects ? ctx.Customers.Include(c => c.Projects) : ctx.Customers;

    var result = await query
      .Where(c => c.Id == id)
      .FirstOrDefaultAsync();

    if (result is null) return Results.NotFound("No customer with that id Exists");

    return Results.Ok(result);
  }

  // Create
  public static async Task<IResult> Post(BillingContext ctx, Customer model)
  {
    try
    {
      ctx.Add(model);

      if (await ctx.SaveAllAsync())
      {
        return Results.CreatedAtRoute("GetOneCustomer", new { id = model.Id }, model);
      }

      return Results.BadRequest("Failed to save new customer.");
    }
    catch (Exception ex)
    {
      return Results.Problem($"Exception thrown while saving customer: {ex.Message}", statusCode: 500);
    }
  }

  // Update
  public static async Task<IResult> Update(BillingContext ctx, int id, Customer model)
  {
    try
    {
      var old = await ctx.Customers.FindAsync(id);

      if (old is null) return Results.NotFound("No customer with that id Exists");

      model.Adapt(old);

      await ctx.SaveAllAsync();

      return Results.Ok(old);
    }
    catch (Exception ex)
    {
      return Results.Problem($"Exception thrown while saving customer: {ex.Message}", statusCode: 500);
    }
  }

  // Delete
  public static async Task<IResult> Delete(BillingContext ctx, int id)
  {
    try
    {
      var old = await ctx.Customers.FindAsync(id);

      if (old is null) return Results.NotFound("No customer with that id Exists");

      ctx.Remove(old);

      if (await ctx.SaveAllAsync())
      {
        return Results.Ok("Deleted.");
      }

      return Results.BadRequest("Failed to save new customer.");
    }
    catch (Exception ex)
    {
      return Results.Problem($"Exception thrown while saving customer: {ex.Message}", statusCode: 500);
    }
  }

}
