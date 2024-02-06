using Microsoft.EntityFrameworkCore;
using Odin.Api.Config;
using Odin.Api.Database;
using Odin.Api.Endpoints;
using Odin.Api.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var env = builder.Environment;

builder.Services.AddCors(options =>
{
    if (env.IsDevelopment())
    {
        options.AddDefaultPolicy(builder =>
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
        );
    }
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new DateTimeOffsetJsonConverter());
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<ITemperatureService, TemperatureService>();
builder.Services.AddScoped<IUnitService, UnitService>();

if (env.IsDevelopment())
{
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
}

var app = builder.Build();

// Seed the database with application data if it has not been seeded yet. A testing env variable guard is used so
// we can decouple application seeding from testing environments if needed.
if (!env.IsTesting())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var unit = dbContext.Units.FirstOrDefault();
    if (unit is null)
    {
        Console.WriteLine("Seed data not found, seeding database...");
        var seeder = new DataSeeder(dbContext);
        seeder.Seed();
        Console.WriteLine("Database seeded successfully.");
    }
    else
    {
        Console.WriteLine("Database already seeded, skipping step.");
    }
}

app.UseCors();

app.UsePathBase("/api/v1");

app.MapGet("/", () => "Hello World!");

app.MapGroup("/devices").MapDeviceEndpoints();
app.MapGroup("/temperatures").MapTemperatureEndpoints();

app.Run();

public partial class Program { }
