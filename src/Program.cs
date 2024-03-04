using WilderMinds.MinimalApiDiscovery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Asp.Versioning;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using System.Reflection;
using RestDesign.Services;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

var svcs = builder.Services;

svcs.AddDbContext<BillingContext>();

svcs.AddExceptionHandler<RestDesignExceptionHandler>();

svcs.AddHttpCacheHeaders();

svcs.AddCors(setup =>
{
  setup.AddDefaultPolicy(cfg =>
  {
    cfg.AllowAnyOrigin();
    cfg.AllowAnyMethod();
    cfg.AllowAnyHeader();
  });

  setup.AddPolicy("Prevent", cfg =>
  {
    cfg.WithOrigins("https://localhost:5002");
  });
});

svcs.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

svcs.AddFluentValidationAutoValidation();

svcs.AddApiVersioning(cfg =>
{
  cfg.ReportApiVersions = true;
  cfg.DefaultApiVersion = new ApiVersion(2, 0);
  cfg.AssumeDefaultVersionWhenUnspecified = true;
  cfg.ApiVersionReader = ApiVersionReader.Combine(
    new AcceptHeaderApiVersionReader(),
    new QueryStringApiVersionReader("v"),
    new HeaderApiVersionReader("X-Version"));
});


svcs.AddOutputCache(cfg => cfg.AddBasePolicy(bldr => bldr.Expire(TimeSpan.FromSeconds(2))));

svcs.ConfigureHttpJsonOptions(opt =>
{
  opt.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

//svcs.AddControllers(cfg =>
//{
//  cfg.RespectBrowserAcceptHeader = true;
//})
//  .AddXmlSerializerFormatters();

svcs.AddRazorPages();

svcs.AddEndpointsApiExplorer();
svcs.AddSwaggerGen(o =>
{
  o.EnableAnnotations();
  o.SwaggerDoc("1.0", new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "Api version  1", Version = "1.0", Description = "Original Version." });
  o.SwaggerDoc("2.0", new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "Api version  2", Version = "2.0", Description = "Updated Version." });

  o.OperationFilter<SwaggerParameterFilters>();
  o.DocumentFilter<SwaggerVersionMapping>();

  o.DocInclusionPredicate((version, desc) =>
  {
    var metadata = desc.ActionDescriptor.EndpointMetadata;

    var versionData = metadata.FirstOrDefault(i => i.GetType() == typeof(ApiVersionMetadata));
    if (versionData is null) return true; // no metadata so it's fine
    var versionNumber = double.Parse(version);
    var data = ((ApiVersionMetadata)versionData).MappingTo(new ApiVersion(versionNumber));
    if (data == ApiVersionMapping.Explicit)
    {
      return true;
    }

    return false;
  });
});

var app = builder.Build();

// Using empty configuration to force it to use IExceptionHandler
app.UseExceptionHandler(_ => { });

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
}

app.UseCors(cfg =>
{
  cfg.AllowAnyHeader();
  cfg.AllowAnyMethod();
  cfg.AllowAnyOrigin();
});

app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI(opt =>
{
  opt.SwaggerEndpoint($"/swagger/1.0/swagger.json", $"v1.0");
  opt.SwaggerEndpoint($"/swagger/2.0/swagger.json", $"v2.0");
});

app.UseRouting();

app.UseHttpCacheHeaders();

app.MapRazorPages();
app.MapApis();

// Seed the database
using var scope = app.Services.CreateScope();
scope.ServiceProvider.GetRequiredService<BillingContext>().Database.EnsureCreated();

app.Run();

partial class Program { }
