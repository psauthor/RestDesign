using DesigningApis.Data;
using DesigningApis;
using WilderMinds.MinimalApiDiscovery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using DesigningApis.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Asp.Versioning;

var builder = WebApplication.CreateBuilder(args);

var svcs = builder.Services;

svcs.AddDbContext<BillingContext>();

svcs.AddHttpCacheHeaders(opt => opt.MaxAge = 600);
svcs.AddResponseCaching();

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
app.UseResponseCaching();
app.UseHttpCacheHeaders();

app.UseSwagger();
app.UseSwaggerUI();

app.MapRazorPages();
app.MapApis();

// Seed the database
using var scope = app.Services.CreateScope();
scope.ServiceProvider.GetRequiredService<BillingContext>().Database.EnsureCreated();

app.Run();

partial class Program { }
