using System.Text;
using AccessTrackAPI;
using AccessTrackAPI.Data;
using AccessTrackAPI.Services; 
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

// adjust the program later 

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext with the connection string from configuration
builder.Services.AddDbContext<AccessControlContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services for controllers
builder.Services.AddControllers();
builder.Services.AddOpenApi(); // Swagger (later)

// Registre o TokenService
builder.Services.AddTransient<TokenService>(); 

// Load the JWT key from configuration
Configuration.JwtKey = builder.Configuration["JwtKey"];

// Configuração da autenticação JWT
var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// Registrar IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

builder.Services.AddMemoryCache();

var app = builder.Build();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// checking the keys (remove later)
Console.WriteLine($"JWT Key: {Configuration.JwtKey}");
Console.WriteLine($"Connection String: {connectionString}");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
    Console.WriteLine("Development Environment");
    // app.MapOpenApi();
}

// Map controller routes
app.MapControllers();

// app.UseHttpsRedirection();

app.Run();