using System.IdentityModel.Tokens.Jwt;
using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScaleTrackAPI.Database;
using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Extensions;
using ScaleTrackAPI.Messages;
using ScaleTrackAPI.Middleware;
using ScaleTrackAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Initialize error and success messages
ErrorMessages.Init(builder.Configuration);
SuccessMessages.Init(builder.Configuration);

// Load environment variables from .env
Env.Load();

// ✅ Add modular services
builder.Services
    .AddDatabase(builder.Configuration)        // DbContext
    .AddIdentityServices(builder.Configuration) // Identity + JWT
    .AddRepositories()                         // Repositories
    .AddValidators()                           // Validators
    .AddHelpers()                              // Helper classes
    .AddDomainServices();                       // Application services

// Add controllers and Swagger
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        opt.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer <token>'"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

// ✅ Disable automatic claim type mapping AFTER services are registered
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var app = builder.Build();

// Apply migrations and seed initial data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    var userManager = services.GetRequiredService<UserManager<User>>();

    context.Database.Migrate();
    await SeedData.Initialize(context, userManager, builder.Configuration);
}

// Middleware
app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

// Optional HTTPS redirect
// app.UseHttpsRedirection();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Run app
app.Run();
