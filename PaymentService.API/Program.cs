using MassTransit;
using PaymentService.API.Application.Interfaces;
using PaymentService.API.Consumers;
using PaymentService.API.Infrastructure.Repositories;
using PaymentService.API.Infrastructure.Messaging;
using PaymentService.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

var rabbitSection = builder.Configuration.GetSection("RabbitMQ");
var rabbitHost = rabbitSection.GetValue<string>("Host") ?? "rabbitmq";
var rabbitVHost = rabbitSection.GetValue<string>("VirtualHost") ?? "/";
var rabbitUser = rabbitSection.GetValue<string>("Username") ?? "guest";
var rabbitPass = rabbitSection.GetValue<string>("Password") ?? "guest";

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Application dependencies
builder.Services.AddSingleton<IPaymentRepository, InMemoryPaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService.API.Application.Services.PaymentService>();

// MassTransit + consumer registration
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitHost, rabbitVHost, h =>
        {
            h.Username(rabbitUser);
            h.Password(rabbitPass);
        });

        // receive endpoint that will get OrderCreatedEvent messages
        cfg.ReceiveEndpoint("order-created-queue", e =>
        {
            e.ConfigureConsumer<OrderCreatedConsumer>(context);
            // configure retries / concurrency if desired
            e.PrefetchCount = 16;
        });
    });
});

// publisher wrapper uses MassTransit IPublishEndpoint
builder.Services.AddScoped<IRabbitMqPublisher, MassTransitRabbitMqPublisher>();

var app = builder.Build();

// Middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment Service API v1");
        c.RoutePrefix = "swagger";
    });
}

app.MapControllers();
app.Run();
