using QweCMS.Infrastructure.Data;
using QweCMS.Core.Settings;
using QweCMS.Infrastructure.Repositories;
using QweCMS.Core.Services;
using QweCMS.Infrastructure.Services;
using QweCMS.Api.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure MongoDB
builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("ConnectionStrings:MongoDB"));

// Add MongoDB context
builder.Services.AddSingleton<MongoDbContext>();

// Add repositories
builder.Services.AddSingleton<IMongoRepository<QweCMS.Core.Entities.MixinEntity>, MongoRepository<QweCMS.Core.Entities.MixinEntity>>(provider =>
{
    var dbContext = provider.GetRequiredService<MongoDbContext>();
    return new MongoRepository<QweCMS.Core.Entities.MixinEntity>(dbContext.Database, "mixins");
});

builder.Services.AddSingleton<IMongoRepository<QweCMS.Core.Entities.SchemaEntity>, MongoRepository<QweCMS.Core.Entities.SchemaEntity>>(provider =>
{
    var dbContext = provider.GetRequiredService<MongoDbContext>();
    return new MongoRepository<QweCMS.Core.Entities.SchemaEntity>(dbContext.Database, "schemas");
});

// Add services
builder.Services.AddScoped<IJsonSchemaValidationService, JsonSchemaValidationService>();
builder.Services.AddScoped<IMixinService, MixinService>();
builder.Services.AddScoped<IMixinRepository, MixinRepository>();
builder.Services.AddScoped<ISchemaService, SchemaService>();
builder.Services.AddScoped<ISchemaRepository, SchemaRepository>();
builder.Services.AddScoped<ISchemaCompositionService, SchemaCompositionService>();
builder.Services.AddScoped<JsonPointerService>();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(QweCMS.Core.Mapping.SchemaMappingProfile));

// Add logging
builder.Services.AddLogging();

// Add global exception filter
builder.Services.AddScoped<GlobalExceptionFilter>();

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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "QweCMS API is running!")
.WithName("Root");

app.MapGet("/health", () => new { status = "healthy", timestamp = DateTime.UtcNow })
.WithName("HealthCheck");

app.Run();
