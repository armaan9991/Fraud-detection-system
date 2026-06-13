using FraudDetection.API.Data;
using FraudDetection.API.Services;
using Microsoft.EntityFrameworkCore;
using FraudDetection.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.OpenApi.Models;
using FraudDetection.API.Middleware;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Hangfire;
using Hangfire.PostgreSql;
using FraudDetection.API.Jobs;
using FraudDetection.API.Infrastructure;
using System.Drawing;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1",new OpenApiInfo
    {
        Title = "Fraud detection API",
        Version = "v1"
    });
    options.AddSecurityDefinition("Bearer",new OpenApiSecurityScheme
    {
        Name="Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT Token"
    });
    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddScoped<ITransactionService , TransactionService>();
builder.Services.AddScoped<IUserService , UserService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<IFraudAlertService, FraudAlertService>();
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<IAdminService,AdminService>();
builder.Services.AddHttpClient<IMLPredictionService,MLPredictionService>();
builder.Services.AddScoped<IAuditLogService,AuditLogService>();
builder.Services.AddScoped<ICacheService,CacheService>();
builder.Services.AddScoped<IRefreshTokenCleanupJob,RefreshTokenCleanupJob>();
builder.Services.AddScoped<IDailyFraudReportJob,DailyFraudReportJob>();
builder.Services.AddScoped<IHighRiskUserDetectionJob,HighRiskUserDetectionJob>();
builder.Services.AddScoped<IEmailService,EmailService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)) 
    };
});

// Rate limiting

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("auth",config =>
    {
        config.PermitLimit = 15;
        config.Window = TimeSpan.FromMinutes(5);
        config.QueueLimit = 0;
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
    options.AddFixedWindowLimiter("api",config =>
    {
        config.PermitLimit = 60;
        config.QueueProcessingOrder = 0;
        config.Window = TimeSpan.FromMinutes(1);
    });

    options.OnRejected = async(context, CancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            success = false,
            message = "Too many requests. Please try again later.",
            retryAfter = "1 minute"
        }, CancellationToken);
    };
});

// Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    options.InstanceName = "FraudDection_";
});

// background jobs
// hangfire were we store , serialize  data
builder.Services.AddHangfire(config => config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                                        .UseSimpleAssemblyNameTypeSerializer() 
                                        .UseRecommendedSerializerSettings()
                                        .UsePostgreSqlStorage(options =>
                                        options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))));
// start hangfire server!
builder.Services.AddHangfireServer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
    policy =>
    {
        policy.WithOrigins("http://localhost:5173",
            builder.Configuration["Frontend:Url"] ?? "http://localhost:5173"
            )
                .AllowAnyHeader()
                .AllowAnyMethod();
    });
});


var app = builder.Build();
app.UseCors("AllowFrontend");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization= [new HangfireAuthorizationFilter()
    ]
});
RecurringJob.AddOrUpdate<IRefreshTokenCleanupJob>("refresh-token-cleanup",job => job.ExecuteAsync(), Cron.Daily);
RecurringJob.AddOrUpdate<IDailyFraudReportJob>("daily-fraud-report",job => job.ExecuteAsync(),"0 0 * * *"); 
RecurringJob.AddOrUpdate<IHighRiskUserDetectionJob>("High-risk-user-detection",job => job.ExecuteAsync(),Cron.Hourly); 
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");
