using Altinn.ResourceRegistry.Core;
using Altinn.ResourceRegistry.Models;
using Altinn.ResourceRegistry.Persistence;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigureServices(builder.Services, builder.Configuration);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();



void ConfigureServices(IServiceCollection services, IConfiguration config)
{
    services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

    });

    services.AddSingleton(config);

    services.AddSingleton<IResourceRegistry, ResourceRegistryService>();
    services.AddSingleton<IResourceRegistryRepository, ResourceRepository>();
    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

}