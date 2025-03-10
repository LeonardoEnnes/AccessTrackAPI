using AccessTrackAPI.Data;
using Microsoft.EntityFrameworkCore;

// @desc: Nothing in the description yet

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AccessControlContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services for controllers
builder.Services.AddControllers();
builder.Services.AddOpenApi(); // Swagger (later)

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Map controller routes
app.MapControllers();

// app.UseHttpsRedirection();

app.Run();