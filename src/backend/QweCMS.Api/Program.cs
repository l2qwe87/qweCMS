using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QweCMS.Infrastructure.Data;
using QweCMS.Core.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure MongoDB
builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("ConnectionStrings:MongoDB"));

// Add MongoDB context
builder.Services.AddSingleton<MongoDbContext>();

// Add logging
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "QweCMS API is running!")
.WithName("Root");

app.MapGet("/health", () => new { status = "healthy", timestamp = DateTime.UtcNow })
.WithName("HealthCheck");

app.Run();
