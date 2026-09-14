using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using ReliabilityWebApi.Data;
using ReliabilityWebApi.HealthChecks;
using ReliabilityWebApi.Middleware;
using ReliabilityWebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Controllers & API Documentation
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Reliability & Resilience Web API",
        Version = "v1",
        Description = "Production-grade demonstration of all reliability, fault tolerance, and resilience patterns with EF Core & SQLite"
    });
});

// 2. EF Core ORM with SQLite (sqlite.db)
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite("Data Source=sqlite.db");
});

// 3. HTTP Context & Downstream Client
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient<IDownstreamService, DownstreamService>();

// 4. Core Reliability Services & Idempotency Store (backed by SQLite via EF Core)
var chaosState = new ChaosSimulationState();
builder.Services.AddSingleton(chaosState);
builder.Services.AddSingleton<IIdempotencyStore, EfCoreIdempotencyStore>();

// 5. Register Polly v8 Resilience Pipelines
using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder.AddConsole());
var resilienceLogger = loggerFactory.CreateLogger("ResiliencePipelines");
builder.Services.AddAppResiliencePipelines(chaosState, resilienceLogger);

// 6. Rate Limiting (Token Bucket)
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddTokenBucketLimiter("token-bucket-policy", opt =>
    {
        opt.TokenLimit = 5;
        opt.QueueLimit = 0;
        opt.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
        opt.TokensPerPeriod = 5;
        opt.AutoReplenishment = true;
    });
});

// 7. Health Checks (Liveness & Readiness Separation)
builder.Services.AddHealthChecks()
    .AddCheck("self-liveness", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Process running"), tags: new[] { "live" })
    .AddCheck<DownstreamHealthCheck>("downstream-dependency", tags: new[] { "ready" });

var app = builder.Build();

// Ensure SQLite Database is created and seeded with initial entities (2-4 items each)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// Configure Middleware Pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ReliabilityWebApi v1");
    c.RoutePrefix = string.Empty; // Serves Swagger UI at application root
});

// A. Deadline Propagation Middleware
app.UseMiddleware<DeadlinePropagationMiddleware>();

// B. Load Shedding Middleware (prevents cascading overload)
app.UseMiddleware<LoadSheddingMiddleware>();

// C. Idempotency Middleware (deduplicates safe and retryable writes via SQLite)
app.UseMiddleware<IdempotencyMiddleware>();

// D. Rate Limiter Middleware
app.UseRateLimiter();

// E. Health Check Endpoints
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

// F. Map API Controllers
app.MapControllers();

app.Run();
