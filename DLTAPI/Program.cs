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

builder.Services.Configure<EmailSettings>(options =>
{
    options.SmtpServer = Environment.GetEnvironmentVariable("EMAIL_SETTINGS__SMTP_SERVER");
    options.Port = int.Parse(Environment.GetEnvironmentVariable("EMAIL_SETTINGS__PORT"));
    options.UseSSL = bool.Parse(Environment.GetEnvironmentVariable("EMAIL_SETTINGS__USE_SSL"));
    options.SenderEmail = Environment.GetEnvironmentVariable("EMAIL_SETTINGS__SENDER_EMAIL");
    options.Password = Environment.GetEnvironmentVariable("EMAIL_SETTINGS__PASSWORD");
});

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddTransient<IEmailSender<IdentityUser>, HelperFunctions.DummyEmailSender>();

builder.Services.AddAuthentication();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("AuthenticatedUser", policy => policy.RequireAuthenticatedUser());
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
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
        Description = "Cookie-based auth - logs in via Identity and sends session cookie"
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhostWithCredentials", policy =>
    {
        // REMOVE ALL LOCALHOST IP'S / LOCALHOSTS FOR PRODUCTION!!
        policy
            .WithOrigins("http://localhost:5279", "http://localhost:4200", "https://shotatevdorashvili.com", "http://127.0.0.1:5500")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.ConfigureApplicationCookie(options =>
{
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

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
});

// builder.Services.Configure<EmailSettings>(
//     builder.Configuration.GetSection("EmailSettings")
// );

builder.Services.AddScoped<IScrapeQuestions, ScrapeQuestions>();
builder.Services.AddScoped<IQuestionRepo, QuestionRepo>();
builder.Services.AddScoped<ILeaderboardRepo, LeaderboardRepo>();
builder.Services.AddScoped<IEmailRepo, EmailRepo>();

builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

var app = builder.Build();

// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<DataContext>();
//     db.Database.Migrate();
// }

// app.MapIdentityApi<IdentityUser>();
app.UseCors("AllowLocalhostWithCredentials");
app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

app.UseIpRateLimiting();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

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
// app.UseHttpsRedirection();

await HelperFunctions.SeedAdminUserAsync(app.Services);

app.Run();