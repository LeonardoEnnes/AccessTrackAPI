using System.Text;
using AccessTrackAPI;
using AccessTrackAPI.Data;
using AccessTrackAPI.Services; 
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
LoadConfiguration(builder);
ConfigureAuthentication(builder);
ConfigureServices(builder);
ConfigureMvc(builder);

// Add services for controllers
builder.Services.AddOpenApi(); // Swagger (later)

// register IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.MapControllers();
// app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
    Console.WriteLine("Development Environment");
    // app.MapOpenApi();
}

Console.Clear();
app.Run();

void ConfigureMvc(WebApplicationBuilder builder)
{
    builder.Services.AddMemoryCache(); // add memory cache support
    builder.Services.AddControllers();
}

void LoadConfiguration(WebApplicationBuilder  builder)
{
    // Load the JWT key from configuration
    Configuration.JwtKey = builder.Configuration["JwtKey"];
}

void ConfigureAuthentication(WebApplicationBuilder builder)
{
    // JWT Auth config
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
}

void ConfigureServices(WebApplicationBuilder builder)
{
    // Configure DbContext with the connection string configuration
    builder.Services.AddDbContext<AccessControlContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    
    // Register the TokenService
    builder.Services.AddTransient<TokenService>(); 
}