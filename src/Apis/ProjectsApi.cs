using System.IO;
using System.Xml.Serialization;
using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using RestDesign.Data.Entities;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using Swashbuckle.AspNetCore.Annotations;
using WilderMinds.MinimalApiDiscovery;

namespace RestDesign.Apis;

// Add XML Accept Header support

public class ProjectsApi : IApi
{
  const string XmlContentType = "application/xml";

  public void Register(IEndpointRouteBuilder builder)
  {
    var group = builder.MapGroup("/api/projects")
      .AddFluentValidationAutoValidation()
      .WithMetadata(
        new SwaggerOperationAttribute(
          summary: "Projects",
          description: "Projects that can be assigned tickets for billing."));

    group.MapGet("", GetAll);
    group.MapGet("{id:int}", GetOne).WithName("GetOneProject");
    group.MapPost("", Post);
    group.MapPut("{id:int}", Update);
    group.MapDelete("{id:int}", Delete);

  }

  // Get All
  public static async Task<IResult> GetAll(HttpContext httpContext, BillingContext ctx)
  {

    var result = await ctx.Projects
      .OrderBy(e => e.ProjectName)
      .ToListAsync();

    if (httpContext.Request.Headers.Accept.Contains(XmlContentType))
    {
      var serializer = new XmlSerializer(result.GetType());
      using var writer = new StringWriter();
      serializer.Serialize(writer, result);
      var xml = writer.ToString();

      httpContext.Response.ContentType = XmlContentType;
      await httpContext.Response.WriteAsync(xml);
      return Results.Ok();
    }

    return Results.Ok(result);
  }

  // Get One
  public static async Task<IResult> GetOne(BillingContext ctx, int id)
  {
    var result = await ctx.Projects.FindAsync(id);

    if (result is null) return Results.NotFound("No Project with that id Exists");

    return Results.Ok(result);
  }

  // Create
  public static async Task<IResult> Post(BillingContext ctx, Project model)
  {
    try
    {
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
  public static async Task<IResult> Update(BillingContext ctx, int id, Project model)
  {
    try
    {
      var old = await ctx.Projects.FindAsync(id);

      if (old is null) return Results.NotFound("No Project with that id Exists");

      model.Adapt(old);

      await ctx.SaveAllAsync();
      return Results.Ok(old);
    }
    catch (Exception ex)
    {
      return Results.Problem($"Exception thrown while saving Project: {ex.Message}", statusCode: 500);
    }
  }

  // Delete
  public static async Task<IResult> Delete(BillingContext ctx, int id)
  {
    try
    {
      var old = await ctx.Projects.FindAsync(id);

      if (old is null) return Results.NotFound("No Project with that id Exists");

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
