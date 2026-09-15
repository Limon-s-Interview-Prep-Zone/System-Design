using MassTransit;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SagaPatternWebApi.Consumers;
using SagaPatternWebApi.Data;
using SagaPatternWebApi.StateMachines;

var builder = WebApplication.CreateBuilder(args);

// 0. Configure OpenTelemetry (Distributed Tracing & Metrics)
var serviceName = "SagaPatternWebApi";
var serviceVersion = "1.0.0";
var otlpEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? "http://localhost:4317";

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(serviceName: serviceName, serviceVersion: serviceVersion)
        .AddAttributes(new Dictionary<string, object>
        {
            ["deployment.environment"] = builder.Environment.EnvironmentName
        }))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation(o =>
            {
                o.RecordException = true;
            })
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation(o =>
            {
                o.SetDbStatementForText = true;
            })
            // 🌟 Captures MassTransit publish, consume, RabbitMQ message transfer, and Saga transitions!
            .AddSource("MassTransit")
            .AddConsoleExporter();

        if (builder.Configuration.GetValue<bool>("OpenTelemetry:EnableOtlp", true))
        {
            tracing.AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint));
        }
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddMeter("MassTransit");

        if (builder.Configuration.GetValue<bool>("OpenTelemetry:EnableOtlp", true))
        {
            metrics.AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint));
        }
    });

// Configure OpenTelemetry Logging (Streams structured logs to Loki)
builder.Logging.AddOpenTelemetry(logging =>
{
    logging.SetResourceBuilder(ResourceBuilder.CreateDefault()
        .AddService(serviceName: serviceName, serviceVersion: serviceVersion)
        .AddAttributes(new Dictionary<string, object>
        {
            ["deployment.environment"] = builder.Environment.EnvironmentName
        }));
    logging.IncludeFormattedMessage = true;
    logging.IncludeScopes = true;

    if (builder.Configuration.GetValue<bool>("OpenTelemetry:EnableOtlp", true))
    {
        logging.AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint));
    }
});

// 1. Configure EF Core with SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=saga.db;Cache=Shared;";
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(connectionString);
});

// 2. Configure MassTransit with RabbitMQ, Saga State Machine, and Transactional Outbox
builder.Services.AddMassTransit(x =>
{
    // Configure Transactional Outbox for zero dual-write message loss
    x.AddEntityFrameworkOutbox<AppDbContext>(o =>
    {
        o.UseSqlite();
        o.UseBusOutbox();
    });

    // Register worker consumers
    x.AddConsumer<OrderCommandConsumer>();
    x.AddConsumer<InventoryCommandConsumer>();
    x.AddConsumer<PaymentCommandConsumer>();

    // Register Saga State Machine & EF Core Saga Repository
    x.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .EntityFrameworkRepository(r =>
        {
            r.ExistingDbContext<AppDbContext>();
            r.UseSqlite();
        });

    // Configure RabbitMQ Transport
    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitHost = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
        var rabbitPort = ushort.Parse(builder.Configuration["RabbitMQ:Port"] ?? "5672");
        var virtualHost = builder.Configuration["RabbitMQ:VirtualHost"] ?? "/";
        var username = builder.Configuration["RabbitMQ:Username"] ?? "guest";
        var password = builder.Configuration["RabbitMQ:Password"] ?? "guest";

        cfg.Host(rabbitHost, rabbitPort, virtualHost, h =>
        {
            h.Username(username);
            h.Password(password);
        });

        cfg.ConfigureEndpoints(context);
    });
});

// 3. Add Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "E-Commerce Saga Pattern API (ASP.NET Core 8 + RabbitMQ + SQLite)",
        Version = "v1",
        Description = "Production-grade implementation of the Saga Pattern with MassTransit Automatonymous State Machine demonstrating Forward Flow and Failure Compensation."
    });
});

var app = builder.Build();

// 4. Ensure SQLite database & Outbox tables are created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    db.Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL;");
}

// 5. Configure HTTP pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Saga Pattern API v1");
    c.RoutePrefix = string.Empty; // Serve Swagger at app root (/)
});

app.MapControllers();

app.Run();
