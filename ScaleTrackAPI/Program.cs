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

// ==========================================================
// ✅ Load configuration from Config/ folder
// ==========================================================
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"Config/appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("Config/success-messages.json", optional: true, reloadOnChange: true)
    .AddJsonFile("Config/error-messages.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// ==========================================================
// ✅ Load environment variables from .env file (for secrets)
// ==========================================================
Env.Load();

// ==========================================================
// ✅ Initialize error and success messages
// ==========================================================
ErrorMessages.Init(builder.Configuration);
SuccessMessages.Init(builder.Configuration);

// ==========================================================
// ✅ Register application services
// ==========================================================
builder.Services
    .AddDatabase(builder.Configuration)          // DbContext
    .AddIdentityServices(builder.Configuration)  // Identity + JWT
    .AddRepositories()                           // Repositories
    .AddValidators()                             // Validators
    .AddHelpers()                                // Helper classes
    .AddDomainServices()                         // Business/application services
    .AddAuditTrails()                            // Audit trail helpers
    .AddBusinessRules();                         // Business rules

// ==========================================================
// ✅ Configure Controllers & JSON options
// ==========================================================
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        opt.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// ==========================================================
// ✅ Swagger setup with JWT auth
// ==========================================================
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
                Reference = new Microsoft.OpenApi.Models.OpenApiReference 
                { 
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, 
                    Id = "Bearer" 
                }
            },
            new string[] {}
        }
    });
});

// ==========================================================
// ✅ JWT claim mapping fix
// ==========================================================
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var app = builder.Build();

// ==========================================================
// ✅ Apply migrations & seed data on startup
// ==========================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    var userManager = services.GetRequiredService<UserManager<User>>();

    context.Database.Migrate();
    await SeedData.Initialize(context, userManager, builder.Configuration);
}

// ==========================================================
// ✅ Middleware pipeline
// ==========================================================
app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

// Optional HTTPS redirect
// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
