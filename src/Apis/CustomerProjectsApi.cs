using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using RestDesign.Data.Entities;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using WilderMinds.MinimalApiDiscovery;

namespace RestDesign.Apis;

public class ProjectsApi : IApi
{
  public void Register(IEndpointRouteBuilder builder)
  {
    var group = builder.MapGroup("/api/customers/{customerId:int}/projects")
      .AddFluentValidationAutoValidation();

    group.MapGet("", GetAll);
    group.MapGet("{id:int}", GetOne).WithName("GetOneProject");
    group.MapPost("", Post);
    group.MapPut("{id:int}", Update);
    group.MapDelete("{id:int}", Delete);

  }

  // Get All
  public static async Task<IResult> GetAll(BillingContext ctx, int customerId)
  {
    var result = await ctx.Projects
      .Where(p => p.CustomerId == customerId)
      .OrderBy(e => e.ProjectName)
      .ToListAsync();

    return Results.Ok(result);
  }

  // Get One
  public static async Task<IResult> GetOne(BillingContext ctx, int customerId, int id)
  {
    var result = await ctx.Projects.FindAsync(id);

    if (result is null) return Results.NotFound("No Project with that id Exists");

    if (result.CustomerId != customerId) return Results.BadRequest("Customer and Project are not related.");

    return Results.Ok(result);
  }

  // Create
  public static async Task<IResult> Post(BillingContext ctx, int customerId, Project model)
  {
    try
    {
      var customer = await ctx.Customers.FindAsync(customerId);

      if (customer is null) return Results.NotFound();

      model.CustomerId = customerId;
      ctx.Add(model);

      if (await ctx.SaveAllAsync())
      {
        return Results.CreatedAtRoute("GetOneProject", new { id = model.Id }, model);
      }

      return Results.BadRequest("Failed to save new Project.");
    }
    catch (Exception ex)
    {
      return Results.Problem($"Exception thrown while saving Project: {ex.Message}", statusCode: 500);
    }
  }

  // Update
  public static async Task<IResult> Update(BillingContext ctx, int customerId, int id, Project model)
  {
    try
    {
      var old = await ctx.Projects.FindAsync(id);

      if (old is null) return Results.NotFound("No Project with that id Exists");

      if (old.CustomerId != customerId) return Results.BadRequest("Customer and Project are not related.");

      model.Adapt(old);

      if (await ctx.SaveAllAsync())
      {
        return Results.Ok(old);
      }

      return Results.BadRequest("Failed to save new Project.");
    }
    catch (Exception ex)
    {
      return Results.Problem($"Exception thrown while saving Project: {ex.Message}", statusCode: 500);
    }
  }

  // Delete
  public static async Task<IResult> Delete(BillingContext ctx, int customerId, int id)
  {
    try
    {
      var old = await ctx.Projects.FindAsync(id);

      if (old is null) return Results.NotFound("No Project with that id Exists");

      if (old.CustomerId != customerId) return Results.BadRequest("Customer and Project are not related.");

      ctx.Remove(old);

      if (await ctx.SaveAllAsync())
      {
        return Results.Ok("Deleted.");
      }

      return Results.BadRequest("Failed to save new Project.");
    }
    catch (Exception ex)
    {
      return Results.Problem($"Exception thrown while saving Project: {ex.Message}", statusCode: 500);
    }
  }

}
