using AuthDemo.Data;
using DLTAPI.interfaces;
using DLTAPI.repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DLTAPI.Helper;
using DLTAPI.models;
using AspNetCoreRateLimit;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

// Email Configuration
builder.Services.Configure<EmailSettings>(options =>
{
    options.SmtpServer = Environment.GetEnvironmentVariable("EMAIL_SETTINGS__SMTP_SERVER");
    options.Port = int.Parse(Environment.GetEnvironmentVariable("EMAIL_SETTINGS__PORT"));
    options.UseSSL = bool.Parse(Environment.GetEnvironmentVariable("EMAIL_SETTINGS__USE_SSL"));
    options.SenderEmail = Environment.GetEnvironmentVariable("EMAIL_SETTINGS__SENDER_EMAIL");
    options.Password = Environment.GetEnvironmentVariable("EMAIL_SETTINGS__PASSWORD");
});

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
});

// Database Context
builder.Services.AddDbContext<DataContext>(options =>
{
    // var connection = builder.Configuration.GetConnectionString("DefaultConnection");
    // options.UseNpgsql(connection);
    var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

    if (string.IsNullOrEmpty(connectionString))
    {
        connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    }
    else
    {
        connectionString = HelperFunctions.ParseRailwayConnectionString(connectionString);
    }

    options.UseNpgsql(connectionString);
});

// Identity Configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

// Cookie Configuration (CRITICAL FIX)
builder.Services.ConfigureApplicationCookie(options =>
{
    // Security Settings
    options.Cookie.Name = ".AspNetCore.Identity.Application";
    // options.Cookie.Domain = "shotatevdorashvili.com";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

    // Expiration Settings
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;

    // Path Settings
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";

    // API Response Handling
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = 403;
        return Task.CompletedTask;
    };
});

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("AuthenticatedUser", policy => policy.RequireAuthenticatedUser());
});


// API Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger Configuration
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
    {
        Title = "Auth",
        Version = "v1"
    });

    options.AddSecurityDefinition("cookieAuth", new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
    {
        Name = ".AspNetCore.Identity.Application",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        In = Microsoft.OpenApi.Models.ParameterLocation.Cookie,
        Description = "Cookie-based auth"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "cookieAuth"
                }
            },
            Array.Empty<string>()
        }
    });
});

// CORS Configuration (UPDATED)
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentCors", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5279",
                "http://localhost:5173",
                "https://shotatevdorashvili.com"
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });

    options.AddPolicy("ProductionCors", policy =>
    {
        policy.WithOrigins(
            "https://shotatevdorashvili.com",
            "https://dlt-api.shotatevdorashvili.com",
            "https://dlt.shotatevdorashvili.com",
            "http://localhost:5173"
            )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Additional Services
builder.Services.AddScoped<IScrapeQuestions, ScrapeQuestions>();
builder.Services.AddScoped<IQuestionRepo, QuestionRepo>();
builder.Services.AddScoped<ILeaderboardRepo, LeaderboardRepo>();
builder.Services.AddScoped<IEmailRepo, EmailRepo>();

// Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

var app = builder.Build();

// Database Migration
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var db = services.GetRequiredService<DataContext>();

    try
    {
        logger.LogInformation("Checking database state...");
        if (!db.Database.CanConnect())
        {
            logger.LogInformation("Database doesn't exist - creating and migrating...");
            db.Database.Migrate();
        }
        else
        {
            var pendingMigrations = db.Database.GetPendingMigrations().ToList();
            if (pendingMigrations.Any())
            {
                logger.LogInformation("Applying {Count} pending migrations: {Migrations}",
                    pendingMigrations.Count, string.Join(", ", pendingMigrations));
                db.Database.Migrate();
            }
            else
            {
                logger.LogInformation("Database is up-to-date");
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error during database migration");
        throw;
    }
}

// Middleware Pipeline
app.UseRouting();

// Use appropriate CORS policy based on environment
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevelopmentCors");
}
else
{
    app.UseCors("ProductionCors");
}

app.UseAuthentication();
app.UseAuthorization();
app.UseIpRateLimiting();

app.MapControllers();
app.MapOpenApi();

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
});

// Custom error responses
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            Status = 401,
            Message = "Unauthorized - Please log in"
        });
    }

    if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            Status = 403,
            Message = "Forbidden - Insufficient permissions"
        });
    }
});

await HelperFunctions.SeedAdminUserAsync(app.Services);

app.Run();