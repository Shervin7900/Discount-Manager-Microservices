using BaseApi.Data;
using BaseApi.Extensions;
using Duende.IdentityServer.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=(localdb)\\mssqllocaldb;Database=DiscountManager;Trusted_Connection=True;MultipleActiveResultSets=true";
var redisConnectionString = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";

builder.Services.AddDatabaseContext(connectionString);
builder.Services.AddIdentityAndIdentityServer();
builder.Services.AddAdvancedCaching(redisConnectionString);
builder.Services.AddSystematicHealthChecks(connectionString, redisConnectionString);
builder.Services.AddVectorMetrics();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddHttpClient();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Base API v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "Base API - API & Dashboards";
        // Adding custom links or description if possible via the UI
    });
}

app.UseHttpsRedirection();

app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();

app.UseVectorMetrics();

app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = HealthChecks.UI.Client.UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecksUI(setup =>
{
    setup.UIPath = "/health-ui"; // UI panel for checking status
});

app.MapControllers();

// Seed IdentityServer data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();

    if (!context.Clients.Any())
    {
        foreach (var client in BaseApi.Data.Config.Clients)
        {
            context.Clients.Add(client.ToEntity());
        }
        context.SaveChanges();
    }

    if (!context.IdentityResources.Any())
    {
        foreach (var resource in BaseApi.Data.Config.IdentityResources)
        {
            context.IdentityResources.Add(resource.ToEntity());
        }
        context.SaveChanges();
    }

    if (!context.ApiScopes.Any())
    {
        foreach (var scopeItem in BaseApi.Data.Config.ApiScopes)
        {
            context.ApiScopes.Add(scopeItem.ToEntity());
        }
        context.SaveChanges();
    }
}

app.Run();
