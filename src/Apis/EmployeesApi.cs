using Asp.Versioning;
using Mapster;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RestDesign.Data.Entities;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using WilderMinds.MinimalApiDiscovery;

namespace RestDesign.Apis;

public class EmployeesApi : IApi
{
  public void Register(IEndpointRouteBuilder builder)
  {
    var group = builder.MapGroup("/api/employees")
      .RequireCors("Prevent")
      .AddFluentValidationAutoValidation();

    group.MapGet("", GetAll);
    group.MapGet("{id:int}", GetOne).WithName("GetOneEmployee");
    group.MapPost("", Post);
    group.MapPut("{id:int}", Update);
    group.MapDelete("{id:int}", Delete);

  }

  // Get All
  [HttpCacheExpiration(NoStore = true, MaxAge = 1)]
  public static async Task<IResult> GetAll(BillingContext ctx)
  {
    var result = await ctx.Employees
      .OrderBy(e => e.Name)
      .ToListAsync();

    return Results.Ok(result);
  }

  // Get One
  [HttpCacheExpiration(NoStore = true, MaxAge = 1)]
  public static async Task<IResult> GetOne(BillingContext ctx, int id)
  {
    var result = await ctx.Employees.FindAsync(id);

    if (result is null) return Results.NotFound("No employee with that id Exists");

    return Results.Ok(result);
  }

  // Create
  public static async Task<IResult> Post(BillingContext ctx, Employee model)
  {
    try
    {
      ctx.Add(model);

      if (await ctx.SaveAllAsync())
      {
        return Results.CreatedAtRoute("GetOneEmployee", new { id = model.Id }, model);
      }

      return Results.BadRequest("Failed to save new employee.");
    }
    catch (Exception ex)
    {
      return Results.Problem($"Exception thrown while saving employee: {ex.Message}", statusCode: 500);
    }
  }

  // Update
  public static async Task<IResult> Update(BillingContext ctx, int id, Employee model)
  {
    try
    {
      var old = await ctx.Employees.FindAsync(id);

      if (old is null) return Results.NotFound("No employee with that id Exists");

      model.Adapt(old);

      if (await ctx.SaveAllAsync())
      {
        return Results.Ok(old);
      }

      return Results.BadRequest("Failed to save new employee.");
    }
    catch (Exception ex)
    {
      return Results.Problem($"Exception thrown while saving employee: {ex.Message}", statusCode: 500);
    }
  }

  // Delete
  public static async Task<IResult> Delete(BillingContext ctx, int id)
  {
    try
    {
      var old = await ctx.Employees.FindAsync(id);

      if (old is null) return Results.NotFound("No employee with that id Exists");

      ctx.Remove(old);

      if (await ctx.SaveAllAsync())
      {
        return Results.Ok("Deleted.");
      }

      return Results.BadRequest("Failed to save new employee.");
    }
    catch (Exception ex)
    {
      return Results.Problem($"Exception thrown while saving employee: {ex.Message}", statusCode: 500);
    }
  }

}
