using Microsoft.EntityFrameworkCore;
using Odin.Api.Converters;
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

app.UseCors();

app.UsePathBase("/api/v1");

app.MapGet("/", () => "Hello World!");

app.MapGroup("/devices").MapDeviceEndpoints();
app.MapGroup("/units").MapUnitEndpoints();

app.Run();

public partial class Program { }
