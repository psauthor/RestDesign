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

var builder = WebApplication.CreateBuilder(args);

var svcs = builder.Services;

svcs.AddDbContext<BillingContext>();

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
svcs.AddSwaggerGen();

var app = builder.Build();

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
app.UseSwaggerUI();

app.UseRouting();

app.UseHttpCacheHeaders();

app.MapRazorPages();
app.MapApis();

// Seed the database
using var scope = app.Services.CreateScope();
scope.ServiceProvider.GetRequiredService<BillingContext>().Database.EnsureCreated();

app.Run();

partial class Program { }
