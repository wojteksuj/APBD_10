using Device.Entities;
using Device.Logic;
using Device.Logic.Models;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<DeviceContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDatabse")));

builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<IDeviceValidator, DeviceValidator>();

builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();           
builder.Services.AddAuthorization();      

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthorization();


app.MapGet("/api/devices", async (IDeviceService service) =>
{
    var result = await service.GetAllAsync();
    return Results.Ok(result);
});

app.MapGet("/api/devices/{id:int}", async (int id, IDeviceService service) =>
{
    var result = await service.GetByIdAsync(id);
    return result is not null ? Results.Ok(result) : Results.NotFound();
});

app.MapPost("/api/devices", async (CreateDeviceDto dto, IDeviceService service) =>
{
    try
    {
        var id = await service.CreateAsync(dto);
        return Results.Created($"/api/devices/{id}", new { id });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPut("/api/devices/{id:int}", async (int id, UpdateDeviceDto dto, IDeviceService service) =>
{
    try
    {
        var success = await service.UpdateAsync(id, dto);
        return success ? Results.NoContent() : Results.NotFound();
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapDelete("/api/devices/{id:int}", async (int id, IDeviceService service) =>
{
    var success = await service.DeleteAsync(id);
    return success ? Results.NoContent() : Results.NotFound();
});

app.Run();
