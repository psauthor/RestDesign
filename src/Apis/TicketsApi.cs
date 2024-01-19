using Asp.Versioning;
using Mapster;
using Marvin.Cache.Headers;
using Marvin.Cache.Headers.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RestDesign.Data.Entities;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using Swashbuckle.AspNetCore.Annotations;
using WilderMinds.MinimalApiDiscovery;

namespace RestDesign.Apis;

// TODO Add Paging

public class TicketsApi : IApi
{
  public void Register(IEndpointRouteBuilder builder)
  {
    var group = builder.MapGroup("/api/tickets")
      .AddFluentValidationAutoValidation()
      .CacheOutput()
      .WithMetadata(
        new SwaggerOperationAttribute(
          summary: "Tickets",
          description: "Time tickets for invoicing Customers."));



    var theGet = group.MapGet("", GetAll);

    group.MapGet("{id:int}", GetOne).WithName("GetOneTicket")
      .AddHttpCacheExpiration(cacheLocation: CacheLocation.Private, maxAge: 99999)
      .AddHttpCacheValidation(mustRevalidate: true);
    group.MapPost("", Post);
    group.MapPut("{id:int}", Update);
    group.MapDelete("{id:int}", Delete);

  }

  // Get All
  public static async Task<IResult> GetAll(HttpContext http, 
    BillingContext ctx, 
    int page = 1, 
    int pageSize = 10, 
    bool useHeaders = false)
  {
    var results = await ctx.Tickets
      .Include(t => t.Employee)
      .Include(t => t.Project)
      .OrderBy(e => e.Date)
      .Skip(pageSize * (page - 1))
      .Take(pageSize)
      .ToListAsync();

    var totalCount = await ctx.Tickets.CountAsync();
    var prevPage = page > 1 ? $"/api/tickets?page={page - 1}&pageSize={pageSize}" : "";
    var nextPage = $"/api/tickets?page={page + 1}&pageSize={pageSize}";

    if (useHeaders)
    {
      http.Response.Headers["X-Pagination-TotalCount"] = $"{totalCount}";
      http.Response.Headers["X-Pagination-PreviousPage"] = prevPage;
      http.Response.Headers["X-Pagination-NextPage"] = nextPage;
      http.Response.Headers["X-Pagination-CurrentPage"] = $"{page}";
      http.Response.Headers["X-Pagination-PageSize"] = $"{pageSize}";

      return Results.Ok(results);
    }
    else
    {
      return Results.Ok(new
      {
        TotalCount = totalCount,
        PrevPage = prevPage,
        NextPage = nextPage,
        CurrentPage = page,
        PageSize = pageSize,
        Results = results,
      });
    }
  }

  // Get One
  public static async Task<IResult> GetOne(BillingContext ctx, int id)
  {
    var result = await ctx.Tickets
      .Include(t => t.Employee)
      .Include(t => t.Project)
      .OrderBy(t => t.Date)
      .Where(t => t.Id == id)
      .FirstOrDefaultAsync();

    if (result is null) return Results.NotFound("No Ticket with that id Exists");

    return Results.Ok(result);
  }

  // Create
  public static async Task<IResult> Post(BillingContext ctx, Ticket model)
  {
    try
    {
      ctx.Add(model);

      if (await ctx.SaveAllAsync())
      {
        var result = await ctx.Tickets
          .Include(t => t.Project)
          .Include(t => t.Employee)
          .Where(t => t.Id == model.Id).FirstAsync();

        return Results.CreatedAtRoute("GetOneTicket", new { id = model.Id }, result);
      }

      return Results.BadRequest("Failed to save new Ticket.");
    }
    catch (Exception ex)
    {
      return Results.Problem($"Exception thrown while saving Ticket: {ex.Message}", statusCode: 500);
    }
  }

  // Update
  public static async Task<IResult> Update(BillingContext ctx, int id, Ticket model)
  {
    try
    {
      var old = await ctx.Tickets.FindAsync(id);

      if (old is null) return Results.NotFound("No Ticket with that id Exists");

      if (old.Id != id) return Results.BadRequest("Id number mismatch");

      // Don't map the types, just the ids
      model.Project = null;
      model.Employee = null;

      model.Adapt(old);

      if (await ctx.SaveAllAsync())
      {
        var result = await ctx.Tickets
          .Include(t => t.Project)
          .Include(t => t.Employee)
          .Where(t => t.Id == model.Id).FirstAsync();

        return Results.Ok(result);
      }

      return Results.BadRequest("Failed to save new Ticket.");
    }
    catch (Exception ex)
    {
      return Results.Problem($"Exception thrown while saving Ticket: {ex.Message}", statusCode: 500);
    }
  }

  // Delete
  public static async Task<IResult> Delete(BillingContext ctx, int id)
  {
    try
    {
      var old = await ctx.Tickets.FindAsync(id);

      if (old is null) return Results.NotFound("No Ticket with that id Exists");

      if (old.Id != id) return Results.BadRequest("Id number mismatch");

      ctx.Remove(old);

      if (await ctx.SaveAllAsync())
      {
        return Results.Ok("Deleted.");
      }

      return Results.BadRequest("Failed to save new Ticket.");
    }
    catch (Exception ex)
    {
      return Results.Problem($"Exception thrown while saving Ticket: {ex.Message}", statusCode: 500);
    }
  }

}
